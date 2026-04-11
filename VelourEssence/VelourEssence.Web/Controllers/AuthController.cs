using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace VelourEssence.Web.Controllers
{
    /// <summary>
    /// Controlador para manejo de autenticación de usuarios
    /// </summary>
        public class AuthController : Controller
        {
            private readonly IServiceUsuario _serviceUsuario;
            private readonly ILogger<AuthController> _logger;

            public AuthController(IServiceUsuario serviceUsuario, ILogger<AuthController> logger)
            {
                _serviceUsuario = serviceUsuario;
                _logger = logger;
            }

            /// <summary>
            /// Muestra el formulario para restablecer la contraseña
            /// </summary>
            [HttpGet]
            [AllowAnonymous]
            public async Task<IActionResult> ResetPassword(string token)
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToAction("Login");
                }
                // Buscar usuario por token
                var usuarios = await _serviceUsuario.ListAsync();
                var usuario = usuarios.FirstOrDefault(u => u.PasswordResetToken == token && u.PasswordResetTokenExpiration > DateTime.UtcNow);
                if (usuario == null)
                {
                    ViewBag.Error = "El enlace de recuperación es inválido o ha expirado.";
                    return View();
                }
                var model = new VelourEssence.Web.Models.ResetPasswordViewModel
                {
                    Token = token,
                    Email = usuario.Correo
                };
                return View(model);
            }

            /// <summary>
            /// Procesa el cambio de contraseña usando el token
            /// </summary>
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ResetPassword(VelourEssence.Web.Models.ResetPasswordViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                // Buscar usuario por correo y token
                var usuarios = await _serviceUsuario.ListAsync();
                var usuario = usuarios.FirstOrDefault(u => u.Correo == model.Email && u.PasswordResetToken == model.Token && u.PasswordResetTokenExpiration > DateTime.UtcNow);
                if (usuario == null)
                {
                    ViewBag.Error = "El enlace de recuperación es inválido o ha expirado.";
                    return View(model);
                }
                // Cambiar la contraseña
                var editarDto = new VelourEssence.Application.DTOs.EditarUsuarioDto
                {
                    IdUsuario = usuario.IdUsuario,
                    NombreUsuario = usuario.NombreUsuario,
                    Correo = usuario.Correo,
                    Contraseña = model.NuevaContraseña,
                    IdRol = usuario.IdRol ?? 0
                };
                await _serviceUsuario.UpdateAsync(editarDto);
                await _serviceUsuario.ClearPasswordResetTokenAsync(usuario.IdUsuario);
                ViewBag.Message = "Tu contraseña ha sido restablecida exitosamente. Ya puedes iniciar sesión.";
                return View();
            }

        /// <summary>
        /// Muestra la página de login
        /// </summary>
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Si ya está autenticado, redirigir
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginDto());
        }

        /// <summary>
        /// Procesa el login del usuario
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            try
            {
                // Buscar y autenticar usuario
                var usuario = await _serviceUsuario.AuthenticateAsync(loginDto.EmailOUsuario, loginDto.Contraseña);

                if (usuario == null)
                {
                    ModelState.AddModelError("", "Credenciales incorrectas.");
                    return View(loginDto);
                }

                // Crear claims del usuario
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Email, usuario.Correo),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = loginDto.RecordarMe,
                    ExpiresUtc = loginDto.RecordarMe ? 
                        DateTimeOffset.UtcNow.AddDays(30) : 
                        DateTimeOffset.UtcNow.AddHours(8)
                };

                // Actualizar último inicio de sesión
                await _serviceUsuario.UpdateLastLoginAsync(usuario.IdUsuario);

                // Autenticar usuario
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("Usuario {Email} ha iniciado sesión exitosamente", usuario.Correo);

                // Redirigir según rol
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Redirigir según el rol
                return usuario.Rol.ToLower() == "administrador" 
                    ? RedirectToAction("Index", "Usuario") 
                    : RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el login del usuario {Email}", loginDto.EmailOUsuario);
                ModelState.AddModelError("", "Ocurrió un error durante el inicio de sesión. Inténtalo de nuevo.");
                return View(loginDto);
            }
        }

        /// <summary>
        /// Muestra la página de registro
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            // Si ya está autenticado, redirigir
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            var registerDto = new RegisterDto();
            
            // Cargar roles disponibles para registro (solo Cliente)
            var roles = await _serviceUsuario.GetRolesAsync();
            registerDto.RolesDisponibles = roles.Where(r => r.Nombre.ToLower() == "cliente").ToList();

            return View(registerDto);
        }

        /// <summary>
        /// Procesa el registro de nuevo usuario
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _serviceUsuario.GetRolesAsync();
                registerDto.RolesDisponibles = roles.Where(r => r.Nombre.ToLower() == "cliente").ToList();
                return View(registerDto);
            }

            try
            {
                // Verificar si el usuario ya existe
                var usuarioExistente = await _serviceUsuario.GetByEmailOrUsernameAsync(registerDto.Correo);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Correo", "Ya existe un usuario con este correo electrónico.");
                    var roles = await _serviceUsuario.GetRolesAsync();
                    registerDto.RolesDisponibles = roles.Where(r => r.Nombre.ToLower() == "cliente").ToList();
                    return View(registerDto);
                }

                usuarioExistente = await _serviceUsuario.GetByEmailOrUsernameAsync(registerDto.NombreUsuario);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("NombreUsuario", "Ya existe un usuario con este nombre de usuario.");
                    var roles = await _serviceUsuario.GetRolesAsync();
                    registerDto.RolesDisponibles = roles.Where(r => r.Nombre.ToLower() == "cliente").ToList();
                    return View(registerDto);
                }

                // Registrar nuevo usuario usando el método del servicio
                var nuevoUsuario = await _serviceUsuario.RegisterAsync(registerDto);

                _logger.LogInformation("Nuevo usuario registrado: {Email}", registerDto.Correo);

                TempData["SuccessMessage"] = "¡Registro exitoso! Ya puedes iniciar sesión con tus credenciales.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el registro del usuario {Email}", registerDto.Correo);
                ModelState.AddModelError("", "Ocurrió un error durante el registro. Inténtalo de nuevo.");
                
                var roles = await _serviceUsuario.GetRolesAsync();
                registerDto.RolesDisponibles = roles.Where(r => r.Nombre.ToLower() == "cliente").ToList();
                return View(registerDto);
            }
        }

        /// <summary>
        /// Cierra la sesión del usuario
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            _logger.LogInformation("Usuario {Email} ha cerrado sesión", userEmail);
            
            TempData["InfoMessage"] = "Has cerrado sesión exitosamente.";
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Página de acceso denegado
        /// </summary>
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// API para validar disponibilidad de correo (AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ValidarCorreoDisponible(string correo)
        {
            try
            {
                var usuario = await _serviceUsuario.GetByEmailOrUsernameAsync(correo);
                return Json(usuario == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando correo {Email}", correo);
                return Json(false);
            }
        }

        /// <summary>
        /// API para validar disponibilidad de nombre de usuario (AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ValidarUsuarioDisponible(string nombreUsuario)
        {
            try
            {
                var usuario = await _serviceUsuario.GetByEmailOrUsernameAsync(nombreUsuario);
                return Json(usuario == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando usuario {Usuario}", nombreUsuario);
                return Json(false);
            }
        }

        /// <summary>
        /// Muestra la página para restablecer contraseña
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Procesa el envío de instrucciones para restablecer contraseña
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError("Email", "El correo electrónico es requerido.");
                return View();
            }

            // Buscar usuario por correo
            var usuario = await _serviceUsuario.GetByEmailAsync(Email);
            if (usuario != null)
            {
                // Mostrar directamente el formulario para cambiar contraseña
                var model = new VelourEssence.Web.Models.ResetPasswordViewModel
                {
                    Email = usuario.Correo,
                    Token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N")
                };
                // Opcional: guardar el token en BD si quieres validarlo en el POST
                await _serviceUsuario.SetPasswordResetTokenAsync(usuario.IdUsuario, model.Token, DateTime.UtcNow.AddHours(2));
                return View("ResetPassword", model);
            }
            ModelState.AddModelError("Email", "El correo electrónico no está registrado.");
            return View();
        }
    }
}
