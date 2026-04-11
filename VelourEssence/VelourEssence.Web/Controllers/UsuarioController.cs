using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using VelourEssence.Application.DTOs;
using VelourEssence.Application.Services.Interfaces;

namespace VelourEssence.Web.Controllers
{
    // Controlador que maneja las vistas relacionadas con usuarios - Solo administradores
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        // Servicio para acceder a los datos de usuarios
        private readonly IServiceUsuario _serviceUsuario;

        // Constructor que recibe el servicio de usuarios por inyección de dependencias
        public UsuarioController(IServiceUsuario serviceUsuario)
        {
            _serviceUsuario = serviceUsuario;
        }

        // Acción que muestra la lista de todos los usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _serviceUsuario.ListAsync();
            return View(usuarios); 
        }

        // Acción que muestra los detalles de un usuario específico por su ID
        public async Task<IActionResult> Details(int id)
        {
            var usuario = await _serviceUsuario.GetByIdAsync(id);

            // Si no se encuentra el usuario, devuelve un error 404
            if (usuario == null)
                return NotFound();

            // Si existe, envía el usuario a la vista para mostrar detalles
            return View(usuario);
        }

        // GET: Muestra el formulario para crear un nuevo usuario
        public async Task<IActionResult> Create()
        {
            await CargarRoles();
            return View();
        }

        // POST: Procesa la creación de un nuevo usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearUsuarioDto crearUsuarioDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _serviceUsuario.CreateAsync(crearUsuarioDto);
                    TempData["SuccessMessage"] = "Usuario creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al crear el usuario");
                }
            }

            await CargarRoles();
            return View(crearUsuarioDto);
        }

        // GET: Muestra el formulario para editar un usuario existente
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _serviceUsuario.GetByIdAsync(id);
            if (usuario == null)
                return NotFound();

            var editarUsuario = new EditarUsuarioDto
            {
                IdUsuario = usuario.IdUsuario,
                NombreUsuario = usuario.NombreUsuario,
                Correo = usuario.Correo,
                IdRol = usuario.IdRol ?? 0,
                UltimoInicioSesion = usuario.UltimoInicioSesion
            };

            await CargarRoles();
            return View(editarUsuario);
        }

        // POST: Procesa la edición de un usuario existente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditarUsuarioDto editarUsuarioDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var resultado = await _serviceUsuario.UpdateAsync(editarUsuarioDto);
                    if (resultado != null)
                    {
                        TempData["SuccessMessage"] = "Usuario actualizado exitosamente";
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError(string.Empty, "No se encontró el usuario");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al actualizar el usuario");
                }
            }

            await CargarRoles();
            return View(editarUsuarioDto);
        }

        // POST: Elimina un usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var resultado = await _serviceUsuario.DeleteAsync(id);
                if (resultado)
                {
                    TempData["SuccessMessage"] = "Usuario eliminado exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se pudo eliminar el usuario";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el usuario";
            }

            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para cargar la lista de roles en el ViewBag
        private async Task CargarRoles()
        {
            var roles = await _serviceUsuario.GetRolesAsync();
            ViewBag.Roles = new SelectList(roles, "IdRol", "NombreRol");
        }

        // API para validar nombre de usuario único (para validación en tiempo real)
        [HttpGet]
        public async Task<JsonResult> ValidarNombreUsuario(string nombreUsuario, int? idUsuario = null)
        {
            var existe = await _serviceUsuario.ExisteNombreUsuarioAsync(nombreUsuario, idUsuario);
            return Json(!existe);
        }

        // API para validar correo único (para validación en tiempo real)
        [HttpGet]
        public async Task<JsonResult> ValidarCorreo(string correo, int? idUsuario = null)
        {
            var existe = await _serviceUsuario.ExisteCorreoAsync(correo, idUsuario);
            return Json(!existe);
        }
    }
}
