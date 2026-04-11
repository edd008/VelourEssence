# 🔐 GUÍA DEL SISTEMA DE AUTENTICACIÓN - VELOUR ESSENCE

## 📋 ÍNDICE
1. [Arquitectura del Sistema](#arquitectura-del-sistema)
2. [Ubicación de Archivos](#ubicación-de-archivos)
3. [Flujo de Autenticación](#flujo-de-autenticación)
4. [Código Clave](#código-clave)
5. [Control de Acceso](#control-de-acceso)
6. [Puntos de Demostración](#puntos-de-demostración)

## 🏗️ ARQUITECTURA DEL SISTEMA

### Patrón Utilizado: **MVC + Cookie Authentication + Role-Based Authorization**

```
Usuario → Vista Login → AuthController → ServiceUsuario → RepositoryUsuario → BD
                ↓
         Cookie de Autenticación → Claims → Autorización por Roles
```

## 📁 UBICACIÓN DE ARCHIVOS

### **🎯 CONTROLADORES**
- **Archivo:** `VelourEssence.Web/Controllers/AuthController.cs`
- **Responsabilidad:** Manejo de login, registro, logout y control de acceso

### **📄 DTOs (Data Transfer Objects)**
- **LoginDto:** `VelourEssence.Application/DTOs/LoginDto.cs`
- **RegisterDto:** `VelourEssence.Application/DTOs/RegisterDto.cs`
- **UsuarioDto:** `VelourEssence.Application/DTOs/UsuarioDto.cs`

### **🔧 SERVICIOS**
- **Interfaz:** `VelourEssence.Application/Services/Interfaces/IServiceUsuario.cs`
- **Implementación:** `VelourEssence.Application/Services/Implementations/ServiceUsuario.cs`

### **🗄️ REPOSITORIO**
- **Interfaz:** `VelourEssence.Infraestructure/Repository/Interfaces/IRepositoryUsuario.cs`
- **Implementación:** `VelourEssence.Infraestructure/Repository/Implementations/RepositoryUsuario.cs`

### **🎨 VISTAS**
- **Login:** `VelourEssence.Web/Views/Auth/Login.cshtml`
- **Registro:** `VelourEssence.Web/Views/Auth/Register.cshtml`
- **Acceso Denegado:** `VelourEssence.Web/Views/Auth/AccessDenied.cshtml`

### **⚙️ CONFIGURACIÓN**
- **Startup:** `VelourEssence.Web/Program.cs`

## 🔄 FLUJO DE AUTENTICACIÓN

### **1. ACCESO A PÁGINA PROTEGIDA**
```
Usuario sin autenticar → Middleware detecta falta de autenticación 
                      → Redirige automáticamente a /Auth/Login
```

### **2. PROCESO DE LOGIN**

#### **Paso 1: Mostrar Formulario**
```csharp
// GET /Auth/Login
public IActionResult Login(string? returnUrl = null)
{
    if (User.Identity?.IsAuthenticated == true)
    {
        return RedirectToAction("Index", "Home");
    }
    
    ViewData["ReturnUrl"] = returnUrl;
    return View(new LoginDto());
}
```

#### **Paso 2: Procesar Credenciales**
```csharp
// POST /Auth/Login
public async Task<IActionResult> Login(LoginDto loginDto, string? returnUrl = null)
{
    if (!ModelState.IsValid)
        return View(loginDto);
    
    // Autenticar usuario
    var usuario = await _serviceUsuario.AuthenticateAsync(loginDto.EmailOUsuario, loginDto.Contraseña);
    
    if (usuario == null)
    {
        ModelState.AddModelError("", "Credenciales inválidas");
        return View(loginDto);
    }
    
    // Crear claims y cookie de autenticación
    await CreateAuthenticationCookie(usuario, loginDto.RecordarMe);
    
    // Actualizar último login
    await _serviceUsuario.UpdateLastLoginAsync(usuario.IdUsuario);
    
    // Redirigir
    return RedirectToReturnUrl(returnUrl);
}
```

### **3. VALIDACIÓN EN EL SERVICIO**

#### **Método de Autenticación Principal:**
```csharp
// En ServiceUsuario.cs
public async Task<UsuarioDto?> AuthenticateAsync(string emailOUsuario, string password)
{
    try
    {
        // 1. Buscar usuario por email o nombre de usuario
        var usuario = await _repositoryUsuario.GetByEmailOrUsernameAsync(emailOUsuario);
        
        // 2. Verificar existencia y estado activo
        if (usuario == null || !usuario.Activo)
        {
            _logger.LogWarning("Intento de login fallido para: {EmailOUsuario}", emailOUsuario);
            return null;
        }
        
        // 3. Verificar contraseña con BCrypt
        if (!BCrypt.Net.BCrypt.Verify(password, usuario.Contraseña))
        {
            _logger.LogWarning("Contraseña incorrecta para usuario: {EmailOUsuario}", emailOUsuario);
            return null;
        }
        
        // 4. Mapear a DTO y retornar
        var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
        _logger.LogInformation("Usuario autenticado exitosamente: {Email}", usuario.Correo);
        
        return usuarioDto;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error durante autenticación de {EmailOUsuario}", emailOUsuario);
        return null;
    }
}
```

### **4. CREACIÓN DE COOKIE DE AUTENTICACIÓN**

```csharp
// En AuthController.cs
private async Task CreateAuthenticationCookie(UsuarioDto usuario, bool recordarMe)
{
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
        new Claim(ClaimTypes.Name, usuario.NombreUsuario),
        new Claim(ClaimTypes.Email, usuario.Correo),
        new Claim(ClaimTypes.Role, usuario.NombreRol),
        new Claim("FullName", $"{usuario.Nombre} {usuario.Apellido}")
    };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    var authProperties = new AuthenticationProperties
    {
        IsPersistent = recordarMe,
        ExpiresUtc = recordarMe ? 
            DateTimeOffset.UtcNow.AddDays(30) : 
            DateTimeOffset.UtcNow.AddHours(24)
    };

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),
        authProperties
    );
}
```

## 🛡️ CONTROL DE ACCESO

### **1. CONFIGURACIÓN EN PROGRAM.CS**
```csharp
// Configuración de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();
```

### **2. ATRIBUTOS DE AUTORIZACIÓN**

#### **Solo Usuarios Autenticados:**
```csharp
[Authorize]
public class ProductoController : Controller
{
    // Todas las acciones requieren usuario autenticado
}
```

#### **Solo Administradores:**
```csharp
[Authorize(Roles = "Administrador")]
public class PromocionController : Controller
{
    // Solo usuarios con rol "Administrador"
}
```

#### **Múltiples Roles:**
```csharp
[Authorize(Roles = "Administrador,Moderador")]
public class ReseñaController : Controller
{
    // Administradores O Moderadores
}
```

### **3. CONTROL EN VISTAS**
```html
<!-- En las vistas Razor -->
@if (User.Identity?.IsAuthenticated == true)
{
    <p>Bienvenido, @User.Identity.Name!</p>
    
    @if (User.IsInRole("Administrador"))
    {
        <a asp-controller="Promocion" asp-action="Index">Gestionar Promociones</a>
    }
}
else
{
    <a asp-controller="Auth" asp-action="Login">Iniciar Sesión</a>
}
```

## 🔧 CÓDIGO CLAVE

### **1. HASH DE CONTRASEÑAS**
```csharp
// Al registrar usuario
public async Task<bool> RegisterAsync(RegisterDto registerDto)
{
    // Hash de la contraseña con BCrypt
    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Contraseña);
    
    var usuario = new Usuario
    {
        // ... otros campos
        Contraseña = hashedPassword,
        FechaCreacion = DateTime.Now,
        Activo = true
    };
    
    return await _repositoryUsuario.CreateAsync(usuario);
}
```

### **2. LOGOUT**
```csharp
public async Task<IActionResult> Logout()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    
    _logger.LogInformation("Usuario {User} ha cerrado sesión", User.Identity?.Name);
    
    TempData["SuccessMessage"] = "Has cerrado sesión exitosamente";
    return RedirectToAction("Index", "Home");
}
```

### **3. MIDDLEWARE DE AUTORIZACIÓN**
```csharp
// En Program.cs - Orden importante
app.UseAuthentication(); // Primero autenticación
app.UseAuthorization();  // Luego autorización
```

## 🎯 PUNTOS DE DEMOSTRACIÓN

### **1. FLUJO COMPLETO DE LOGIN**
- Acceder a página protegida sin autenticar
- Redirección automática a login
- Introducir credenciales válidas
- Redirección a página original
- Mostrar información del usuario logueado

### **2. CONTROL DE ROLES**
- Login como usuario normal
- Intentar acceder a área de administrador
- Mostrar página de "Acceso Denegado"
- Login como administrador
- Acceso exitoso a todas las áreas

### **3. VALIDACIONES DE SEGURIDAD**
- Intentar login con credenciales inválidas
- Mostrar mensajes de error apropiados
- Demostrar que las contraseñas están hasheadas en BD
- Funcionalidad "Recordarme"

### **4. ESTADO DE SESIÓN**
- Información del usuario en toda la aplicación
- Logout y limpieza de sesión
- Expiración automática de sesión

## 🔍 PUNTOS CLAVE PARA EL PROFESOR

### **CONCEPTOS IMPLEMENTADOS:**
1. **Cookie Authentication:** Gestión de sesiones stateful
2. **Claims-based Identity:** Información del usuario en claims
3. **Role-based Authorization:** Control de acceso por roles
4. **Password Hashing:** BCrypt para seguridad de contraseñas
5. **Redirect After Login:** Funcionalidad de returnUrl
6. **Remember Me:** Persistencia opcional de sesión
7. **Logging:** Auditoría de accesos y errores
8. **Validation:** Multiple layers de validación

### **PATRONES DE DISEÑO:**
- **Repository Pattern:** Acceso a datos
- **Service Layer:** Lógica de negocio
- **Dependency Injection:** Inversión de control
- **DTO Pattern:** Transferencia segura de datos

### **SEGURIDAD:**
- Contraseñas nunca en texto plano
- Cookies HttpOnly y Secure
- Validación en múltiples capas
- Logging de eventos de seguridad
- Protección contra ataques comunes

---

**Fecha de creación:** $(Get-Date)
**Proyecto:** VelourEssence - Sistema de Perfumería
**Autor:** Equipo de Desarrollo
