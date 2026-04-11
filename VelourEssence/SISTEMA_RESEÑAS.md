# 📝 GUÍA DEL SISTEMA DE RESEÑAS - VELOUR ESSENCE

## 📋 ÍNDICE
1. [Arquitectura del Sistema](#arquitectura-del-sistema)
2. [Ubicación de Archivos](#ubicación-de-archivos)
3. [Modelo de Datos](#modelo-de-datos)
4. [Operaciones CRUD](#operaciones-crud)
5. [Validaciones de Negocio](#validaciones-de-negocio)
6. [Control de Permisos](#control-de-permisos)
7. [Código Detallado](#código-detallado)

## 🏗️ ARQUITECTURA DEL SISTEMA

### Patrón: **Clean Architecture + Repository + Service Layer**

```
Vista → Controlador → Servicio → Repositorio → Base de Datos
  ↑         ↓          ↓           ↓
DTOs ← AutoMapper ← Entidades ← Entity Framework
```

## 📁 UBICACIÓN DE ARCHIVOS

### **🗄️ MODELO DE DATOS**
- **Entidad:** `VelourEssence.Infraestructure/Models/Reseña.cs`
- **Propiedades:** IdReseña, Puntuacion, Comentario, FechaCreacion, IdUsuario, IdProducto

### **📄 DTOs (Data Transfer Objects)**
- **Para Mostrar:** `VelourEssence.Application/DTOs/ReseñaDto.cs`
- **Para Crear/Editar:** `VelourEssence.Application/DTOs/CrearReseñaDto.cs`

### **🔧 CAPA DE SERVICIO**
- **Interfaz:** `VelourEssence.Application/Services/Interfaces/IServiceReseña.cs`
- **Implementación:** `VelourEssence.Application/Services/Implementations/ServiceReseña.cs`

### **🗄️ CAPA DE REPOSITORIO**
- **Interfaz:** `VelourEssence.Infraestructure/Repository/Interfaces/IRepositoryReseña.cs`
- **Implementación:** `VelourEssence.Infraestructure/Repository/Implementations/RepositoryReseña.cs`

### **🎮 CONTROLADOR WEB**
- **Controlador:** `VelourEssence.Web/Controllers/ReseñaController.cs`

### **🎨 VISTAS RAZOR**
- **Lista:** `VelourEssence.Web/Views/Reseña/Index.cshtml`
- **Detalle:** `VelourEssence.Web/Views/Reseña/Details.cshtml`
- **Crear:** `VelourEssence.Web/Views/Reseña/Create.cshtml`
- **Editar:** `VelourEssence.Web/Views/Reseña/Edit.cshtml`
- **Eliminar:** `VelourEssence.Web/Views/Reseña/Delete.cshtml`

### **🔄 AUTOMAPPER**
- **Perfil:** `VelourEssence.Application/Profiles/ReseñaProfile.cs`

## 📊 MODELO DE DATOS

### **Entidad Reseña**
```csharp
public partial class Reseña
{
    public int IdReseña { get; set; }
    public int Puntuacion { get; set; }        // 1-5 estrellas
    public string? Comentario { get; set; }    // Texto opcional
    public DateTime FechaCreacion { get; set; } // Auto-generado
    public int IdUsuario { get; set; }         // FK Usuario
    public int IdProducto { get; set; }        // FK Producto

    // Navegación
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
```

### **DTOs Principales**

#### **ReseñaDto (Para Mostrar)**
```csharp
public record ReseñaDto
{
    public int IdReseña { get; set; }
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int IdUsuario { get; set; }
    public int IdProducto { get; set; }
    
    // Propiedades calculadas
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreProducto { get; set; } = string.Empty;
    public string FechaFormateada => FechaCreacion.ToString("dd/MM/yyyy");
    public string EstrellasTxt => new string('⭐', Puntuacion);
}
```

#### **CrearReseñaDto (Para Crear/Editar)**
```csharp
public record CrearReseñaDto
{
    public int IdReseña { get; set; }
    
    [Required(ErrorMessage = "La puntuación es obligatoria")]
    [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5")]
    public int Puntuacion { get; set; }
    
    [MaxLength(1000, ErrorMessage = "El comentario no puede exceder 1000 caracteres")]
    public string? Comentario { get; set; }
    
    [Required(ErrorMessage = "Debe seleccionar un producto")]
    public int IdProducto { get; set; }
    
    public int IdUsuario { get; set; } // Se asigna automáticamente
    
    // Para dropdowns
    public string? NombreProducto { get; set; }
}
```

## 🔄 OPERACIONES CRUD

### **1. LISTAR RESEÑAS (READ)**

#### **Flujo Completo:**
```
GET /Reseña → ReseñaController.Index() → ServiceReseña.ListAsync() → RepositoryReseña.ListAsync() → BD
```

#### **Código del Controlador:**
```csharp
[Authorize]
public async Task<IActionResult> Index()
{
    try
    {
        var reseñas = await _serviceReseña.ListAsync();
        return View(reseñas);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al cargar lista de reseñas");
        TempData["ErrorMessage"] = "Error al cargar las reseñas";
        return View(new List<ReseñaDto>());
    }
}
```

#### **Código del Servicio:**
```csharp
public async Task<ICollection<ReseñaDto>> ListAsync()
{
    try
    {
        var reseñas = await _repositoryReseña.ListAsync();
        return _mapper.Map<ICollection<ReseñaDto>>(reseñas);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error en ServiceReseña.ListAsync");
        throw;
    }
}
```

#### **Código del Repositorio:**
```csharp
public async Task<ICollection<Reseña>> ListAsync()
{
    return await _context.Reseña
        .Include(r => r.IdUsuarioNavigation)
        .Include(r => r.IdProductoNavigation)
        .OrderByDescending(r => r.FechaCreacion)
        .ToListAsync();
}
```

### **2. VER DETALLE (READ)**

#### **Flujo:**
```
GET /Reseña/Details/{id} → ReseñaController.Details(id) → ServiceReseña.GetByIdAsync(id)
```

#### **Código del Controlador:**
```csharp
public async Task<IActionResult> Details(int id)
{
    var reseña = await _serviceReseña.GetByIdAsync(id);
    
    if (reseña == null)
    {
        TempData["ErrorMessage"] = "Reseña no encontrada";
        return RedirectToAction(nameof(Index));
    }
    
    return View(reseña);
}
```

### **3. CREAR RESEÑA (CREATE)**

#### **GET - Mostrar Formulario:**
```csharp
public async Task<IActionResult> Create(int? idProducto = null)
{
    var model = new CrearReseñaDto();
    
    if (idProducto.HasValue)
    {
        model.IdProducto = idProducto.Value;
        var producto = await _serviceProducto.GetByIdAsync(idProducto.Value);
        model.NombreProducto = producto?.Nombre;
    }
    
    // Cargar lista de productos para dropdown
    ViewBag.Productos = await GetProductosSelectList();
    
    return View(model);
}
```

#### **POST - Procesar Formulario:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(CrearReseñaDto reseñaDto)
{
    if (!ModelState.IsValid)
    {
        ViewBag.Productos = await GetProductosSelectList();
        return View(reseñaDto);
    }
    
    try
    {
        // Asignar usuario actual
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userId, out int parsedUserId))
        {
            reseñaDto.IdUsuario = parsedUserId;
        }
        
        var success = await _serviceReseña.CreateAsync(reseñaDto);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Reseña creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        
        ModelState.AddModelError("", "Error al crear la reseña");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al crear reseña");
        ModelState.AddModelError("", "Error inesperado al crear la reseña");
    }
    
    ViewBag.Productos = await GetProductosSelectList();
    return View(reseñaDto);
}
```

### **4. EDITAR RESEÑA (UPDATE)**

#### **GET - Cargar Datos para Editar:**
```csharp
public async Task<IActionResult> Edit(int id)
{
    var reseña = await _serviceReseña.GetForEditAsync(id);
    
    if (reseña == null)
    {
        TempData["ErrorMessage"] = "Reseña no encontrada";
        return RedirectToAction(nameof(Index));
    }
    
    // Verificar permisos
    if (!CanEditReseña(reseña))
    {
        TempData["ErrorMessage"] = "No tienes permisos para editar esta reseña";
        return RedirectToAction(nameof(Index));
    }
    
    ViewBag.Productos = await GetProductosSelectList();
    return View(reseña);
}
```

#### **POST - Procesar Edición:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(CrearReseñaDto reseñaDto)
{
    if (!ModelState.IsValid)
    {
        ViewBag.Productos = await GetProductosSelectList();
        return View(reseñaDto);
    }
    
    // Verificar permisos nuevamente
    var existingReseña = await _serviceReseña.GetForEditAsync(reseñaDto.IdReseña);
    if (existingReseña == null || !CanEditReseña(existingReseña))
    {
        return Forbid();
    }
    
    var success = await _serviceReseña.UpdateAsync(reseñaDto);
    
    if (success)
    {
        TempData["SuccessMessage"] = "Reseña actualizada exitosamente";
        return RedirectToAction(nameof(Index));
    }
    
    ModelState.AddModelError("", "Error al actualizar la reseña");
    ViewBag.Productos = await GetProductosSelectList();
    return View(reseñaDto);
}
```

### **5. ELIMINAR RESEÑA (DELETE)**

#### **GET - Confirmar Eliminación:**
```csharp
public async Task<IActionResult> Delete(int id)
{
    var reseña = await _serviceReseña.GetByIdAsync(id);
    
    if (reseña == null)
    {
        TempData["ErrorMessage"] = "Reseña no encontrada";
        return RedirectToAction(nameof(Index));
    }
    
    // Verificar permisos
    if (!CanDeleteReseña(reseña))
    {
        return Forbid();
    }
    
    return View(reseña);
}
```

#### **POST - Confirmar y Eliminar:**
```csharp
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var success = await _serviceReseña.DeleteAsync(id);
    
    if (success)
    {
        TempData["SuccessMessage"] = "Reseña eliminada exitosamente";
    }
    else
    {
        TempData["ErrorMessage"] = "Error al eliminar la reseña";
    }
    
    return RedirectToAction(nameof(Index));
}
```

## ✅ VALIDACIONES DE NEGOCIO

### **Validaciones en el Servicio:**

```csharp
public async Task<bool> CreateAsync(CrearReseñaDto reseñaDto)
{
    try
    {
        // 1. Validar que el usuario existe y está activo
        var usuario = await _repositoryUsuario.GetByIdAsync(reseñaDto.IdUsuario);
        if (usuario == null || !usuario.Activo)
        {
            _logger.LogWarning("Intento de crear reseña con usuario inválido: {UserId}", reseñaDto.IdUsuario);
            return false;
        }
        
        // 2. Validar que el producto existe
        var producto = await _repositoryProducto.GetByIdAsync(reseñaDto.IdProducto);
        if (producto == null)
        {
            _logger.LogWarning("Intento de crear reseña para producto inexistente: {ProductId}", reseñaDto.IdProducto);
            return false;
        }
        
        // 3. Verificar que el usuario no haya reseñado ya este producto
        var yaReseñado = await _repositoryReseña.UserHasReviewForProductAsync(reseñaDto.IdUsuario, reseñaDto.IdProducto);
        if (yaReseñado)
        {
            _logger.LogWarning("Usuario {UserId} ya reseñó el producto {ProductId}", reseñaDto.IdUsuario, reseñaDto.IdProducto);
            return false;
        }
        
        // 4. Crear la reseña
        var reseña = _mapper.Map<Reseña>(reseñaDto);
        reseña.FechaCreacion = DateTime.Now;
        
        var result = await _repositoryReseña.CreateAsync(reseña);
        
        if (result)
        {
            _logger.LogInformation("Reseña creada exitosamente por usuario {UserId} para producto {ProductId}", 
                reseñaDto.IdUsuario, reseñaDto.IdProducto);
        }
        
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al crear reseña para usuario {UserId} y producto {ProductId}", 
            reseñaDto.IdUsuario, reseñaDto.IdProducto);
        return false;
    }
}
```

### **Validación de Duplicados en Repositorio:**
```csharp
public async Task<bool> UserHasReviewForProductAsync(int userId, int productId)
{
    return await _context.Reseña
        .AnyAsync(r => r.IdUsuario == userId && r.IdProducto == productId);
}
```

## 🔒 CONTROL DE PERMISOS

### **Reglas de Autorización:**

1. **Usuarios Autenticados:** Pueden ver y crear reseñas
2. **Propietarios:** Pueden editar sus propias reseñas
3. **Administradores:** Pueden gestionar todas las reseñas

### **Implementación en Controlador:**

```csharp
private bool CanEditReseña(CrearReseñaDto reseña)
{
    // Administradores pueden editar cualquier reseña
    if (User.IsInRole("Administrador"))
        return true;
    
    // Usuarios pueden editar solo sus propias reseñas
    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (int.TryParse(currentUserId, out int userId))
    {
        return reseña.IdUsuario == userId;
    }
    
    return false;
}

private bool CanDeleteReseña(ReseñaDto reseña)
{
    // Solo administradores pueden eliminar reseñas
    return User.IsInRole("Administrador");
}
```

### **Control en Vistas:**
```html
<!-- En Index.cshtml -->
@foreach (var reseña in Model)
{
    <div class="reseña-card">
        <div class="reseña-actions">
            @if (User.IsInRole("Administrador") || reseña.IdUsuario.ToString() == User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            {
                <a asp-action="Edit" asp-route-id="@reseña.IdReseña" class="btn btn-warning">Editar</a>
            }
            
            @if (User.IsInRole("Administrador"))
            {
                <a asp-action="Delete" asp-route-id="@reseña.IdReseña" class="btn btn-danger">Eliminar</a>
            }
        </div>
    </div>
}
```

## 🔄 AUTOMAPPER CONFIGURATION

### **Perfil de Mapeo:**
```csharp
public class ReseñaProfile : Profile
{
    public ReseñaProfile()
    {
        // Reseña → ReseñaDto
        CreateMap<Reseña, ReseñaDto>()
            .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => 
                src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.NombreUsuario : string.Empty))
            .ForMember(dest => dest.NombreProducto, opt => opt.MapFrom(src => 
                src.IdProductoNavigation != null ? src.IdProductoNavigation.Nombre : string.Empty));
        
        // CrearReseñaDto → Reseña
        CreateMap<CrearReseñaDto, Reseña>()
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore()) // Se asigna en el servicio
            .ForMember(dest => dest.IdUsuarioNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdProductoNavigation, opt => opt.Ignore());
        
        // Reseña → CrearReseñaDto (para editar)
        CreateMap<Reseña, CrearReseñaDto>()
            .ForMember(dest => dest.NombreProducto, opt => opt.MapFrom(src => 
                src.IdProductoNavigation != null ? src.IdProductoNavigation.Nombre : string.Empty));
    }
}
```

## 🎯 PUNTOS DE DEMOSTRACIÓN

### **1. CRUD Completo**
- Crear una nueva reseña
- Editar reseña propia
- Intentar editar reseña ajena (error de permisos)
- Eliminar reseña como administrador

### **2. Validaciones**
- Intentar crear reseña duplicada
- Validar campos obligatorios
- Validar rango de puntuación (1-5)

### **3. Control de Acceso**
- Login como usuario normal vs administrador
- Diferencias en permisos de edición/eliminación

### **4. Experiencia de Usuario**
- Mensajes de éxito/error
- Formularios user-friendly
- Navegación intuitiva

## 🔍 PUNTOS CLAVE PARA EL PROFESOR

### **PATRONES IMPLEMENTADOS:**
1. **Repository Pattern:** Separación acceso a datos
2. **Service Layer Pattern:** Encapsulación lógica de negocio
3. **DTO Pattern:** Transferencia segura de datos
4. **Mapper Pattern:** AutoMapper para conversiones
5. **Authorization Pattern:** Control de acceso granular

### **PRINCIPIOS SOLID:**
- **S:** Cada clase tiene una responsabilidad específica
- **O:** Abierto para extensión, cerrado para modificación
- **L:** Sustitución de interfaces
- **I:** Interfaces segregadas por funcionalidad
- **D:** Inversión de dependencias con inyección

### **BUENAS PRÁCTICAS:**
- Validación en múltiples capas
- Logging comprehensive
- Manejo de errores robusto
- Separación clara de responsabilidades
- Autorización granular por usuario y rol

---

**Fecha de creación:** $(Get-Date)
**Proyecto:** VelourEssence - Sistema de Perfumería
**Autor:** Equipo de Desarrollo
