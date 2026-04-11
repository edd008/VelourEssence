# Sistema de Promociones - VelourEssence

## Índice
1. [Descripción General](#descripción-general)
2. [Arquitectura del Sistema](#arquitectura-del-sistema)
3. [Modelos y DTOs](#modelos-y-dtos)
4. [Servicios y Repositorios](#servicios-y-repositorios)
5. [Controladores y Vistas](#controladores-y-vistas)
6. [Lógica de Negocio](#lógica-de-negocio)
7. [Validaciones y Control de Acceso](#validaciones-y-control-de-acceso)
8. [Estilos y Experiencia de Usuario](#estilos-y-experiencia-de-usuario)
9. [Puntos Clave para Demostración](#puntos-clave-para-demostración)
10. [Archivos de Referencia](#archivos-de-referencia)

## Descripción General

El sistema de promociones de VelourEssence permite a los administradores gestionar descuentos aplicables tanto a productos específicos como a categorías completas. El sistema calcula automáticamente los precios con descuento y aplica la mejor promoción disponible para cada producto.

### Características Principales:
- **CRUD completo**: Crear, leer, actualizar y eliminar promociones
- **Promociones por producto**: Descuentos específicos para productos individuales
- **Promociones por categoría**: Descuentos aplicables a todos los productos de una categoría
- **Prioridad inteligente**: Sistema que aplica automáticamente la mejor promoción disponible
- **Control de fechas**: Validación de vigencia de promociones
- **Interfaz administrativa**: Formularios y vistas optimizadas para administradores

## Arquitectura del Sistema

El sistema sigue la arquitectura en capas establecida en VelourEssence:

```
Presentation Layer (Views/Controllers)
    ↓
Application Layer (Services/DTOs)
    ↓
Infrastructure Layer (Repository/Data)
    ↓
Database (Entity Framework)
```

### Flujo de Datos:
1. **Usuario administrador** accede a las vistas de promociones
2. **Controller** recibe las peticiones y coordina operaciones
3. **Service Layer** maneja la lógica de negocio y validaciones
4. **Repository Layer** gestiona el acceso a datos
5. **AutoMapper** convierte entre entidades y DTOs
6. **Entity Framework** maneja la persistencia en base de datos

## Modelos y DTOs

### Modelo Principal
**Archivo**: `VelourEssence.Infraestructure/Models/Promocion.cs`

```csharp
public class Promocion
{
    public int PromocionId { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal PorcentajeDescuento { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activa { get; set; }
    
    // Relaciones
    public int? ProductoId { get; set; }  // Para promociones específicas
    public int? CategoriaId { get; set; } // Para promociones por categoría
    public virtual Producto Producto { get; set; }
    public virtual Categoria Categoria { get; set; }
}
```

### DTOs Principales

**PromocionDto** - `VelourEssence.Application/DTOs/PromocionDto.cs`
- DTO para mostrar promociones con nombres de producto/categoría
- Incluye propiedades calculadas como `NombreProducto` y `NombreCategoria`

**CrearPromocionDto** - `VelourEssence.Application/DTOs/CrearPromocionDto.cs`
- DTO para crear/editar promociones
- Incluye validaciones de rango y fechas

## Servicios y Repositorios

### Interfaz del Servicio
**Archivo**: `VelourEssence.Application/Services/Interfaces/IServicePromocion.cs`

```csharp
public interface IServicePromocion
{
    Task<IEnumerable<PromocionDto>> GetAllPromocionesAsync();
    Task<PromocionDto> GetPromocionByIdAsync(int id);
    Task<PromocionDto> CreatePromocionAsync(CrearPromocionDto promocionDto);
    Task<PromocionDto> UpdatePromocionAsync(int id, CrearPromocionDto promocionDto);
    Task<bool> DeletePromocionAsync(int id);
    Task<IEnumerable<PromocionDto>> GetPromocionesActivasAsync();
    Task<IEnumerable<PromocionDto>> GetPromocionesByProductoAsync(int productoId);
    Task<IEnumerable<PromocionDto>> GetPromocionesByCategoriaAsync(int categoriaId);
}
```

### Implementación del Servicio
**Archivo**: `VelourEssence.Application/Services/Implementations/ServicePromocion.cs`

**Funcionalidades Clave:**
- **Validación de fechas**: Verifica que fecha de inicio sea anterior a fecha de fin
- **Validación de producto/categoría**: Ensure que se especifique uno u otro, no ambos
- **Mapeo automático**: Utiliza AutoMapper para conversiones entre entidades y DTOs
- **Gestión de errores**: Manejo apropiado de excepciones y casos no encontrados

### Repositorio
**Archivo**: `VelourEssence.Infraestructure/Repository/Implementations/RepositoryPromocion.cs`

**Características:**
- **Include automático**: Carga relaciones con Producto y Categoria
- **Filtros inteligentes**: Métodos para obtener promociones activas y por criterios específicos
- **Queries optimizadas**: Uso de Entity Framework con includes para evitar N+1 queries

## Controladores y Vistas

### Controlador Principal
**Archivo**: `VelourEssence.Web/Controllers/PromocionController.cs`

**Atributos de Seguridad:**
```csharp
[Authorize(Roles = "Administrador")]
public class PromocionController : Controller
```

**Acciones Principales:**
- **Index**: Lista todas las promociones con paginación
- **Details**: Muestra detalles de una promoción específica
- **Create**: Formulario de creación con validaciones
- **Edit**: Formulario de edición con datos pre-cargados
- **Delete**: Confirmación y eliminación de promociones

### Vistas Principales

**Index** - `VelourEssence.Web/Views/Promocion/Index.cshtml`
- Tabla responsive con información de promociones
- Indicadores visuales de estado (activa/inactiva)
- Botones de acción (editar, ver, eliminar)
- Filtros por estado y fecha

**Create/Edit** - `VelourEssence.Web/Views/Promocion/Create.cshtml` y `Edit.cshtml`
- Formularios con validación client-side y server-side
- Comboboxes para selección de producto o categoría
- DatePickers para fechas de vigencia
- Validación de porcentajes (0-100%)

## Lógica de Negocio

### Cálculo de Precios con Descuento

**Archivo**: `VelourEssence.Application/Profiles/ProductoProfile.cs`

```csharp
.ForMember(dest => dest.PrecioConDescuento, opt => opt.MapFrom(src => 
    src.Promociones.Any(p => p.Activa && p.FechaInicio <= DateTime.Now && p.FechaFin >= DateTime.Now) ?
    src.Precio * (1 - src.Promociones
        .Where(p => p.Activa && p.FechaInicio <= DateTime.Now && p.FechaFin >= DateTime.Now)
        .Max(p => p.PorcentajeDescuento) / 100) :
    src.Categoria.Promociones.Any(p => p.Activa && p.FechaInicio <= DateTime.Now && p.FechaFin >= DateTime.Now) ?
    src.Precio * (1 - src.Categoria.Promociones
        .Where(p => p.Activa && p.FechaInicio <= DateTime.Now && p.FechaFin >= DateTime.Now)
        .Max(p => p.PorcentajeDescuento) / 100) :
    src.Precio))
```

### Prioridad de Promociones:
1. **Promociones directas del producto** (máxima prioridad)
2. **Promociones de la categoría del producto** (si no hay promociones directas)
3. **Precio original** (si no hay promociones aplicables)

### Validaciones de Negocio:
- Una promoción puede aplicar a un producto **O** a una categoría, no a ambos
- Las fechas de inicio deben ser anteriores a las fechas de fin
- Los porcentajes de descuento deben estar entre 0% y 100%
- Solo las promociones activas y dentro del rango de fechas se aplican

## Validaciones y Control de Acceso

### Control de Acceso
- **Restricción por rol**: Solo administradores pueden gestionar promociones
- **Autorización en Controller**: `[Authorize(Roles = "Administrador")]`
- **Validación en vistas**: Botones y enlaces solo visibles para administradores

### Validaciones de Datos

**En DTOs:**
```csharp
[Required(ErrorMessage = "El nombre es requerido")]
[StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
public string Nombre { get; set; }

[Range(0.01, 100, ErrorMessage = "El descuento debe estar entre 0.01% y 100%")]
public decimal PorcentajeDescuento { get; set; }
```

**En Servicios:**
- Validación de fechas lógicas
- Verificación de existencia de producto/categoría
- Validación de unicidad de nombres
- Validación de estado activo/inactivo

## Estilos y Experiencia de Usuario

### Archivos CSS Específicos

**Forms.css** - `VelourEssence.Web/wwwroot/css/forms.css`
- Estilos globales para formularios
- Paleta corporativa (dorado #D4AF37, negro #000000)
- Efectos hover y focus
- Responsive design

**Promocion-index.css** - `VelourEssence.Web/wwwroot/css/promocion-index.css`
- Estilos específicos para la lista de promociones
- Indicadores de estado visual
- Botones de acción estilizados
- Tabla responsive

### Características UX:
- **Feedback visual**: Estados activos/inactivos claramente diferenciados
- **Validación en tiempo real**: Mensajes de error inmediatos
- **Navegación intuitiva**: Breadcrumbs y botones de acción claros
- **Responsive**: Adaptable a diferentes tamaños de pantalla

## Puntos Clave para Demostración

### 1. Creación de Promoción por Producto
**Demostrar:**
- Acceso a `/Promocion/Create`
- Selección de un producto específico
- Configuración de fechas y porcentaje
- Guardado y verificación en lista

### 2. Creación de Promoción por Categoría
**Demostrar:**
- Selección de categoría en lugar de producto
- Impacto en todos los productos de la categoría
- Cálculo automático de precios con descuento

### 3. Visualización de Precios con Descuento
**Demostrar:**
- Navegación a catálogo de productos
- Verificación de precios con descuento aplicado
- Comparación precio original vs. precio con promoción

### 4. Gestión de Estados y Fechas
**Demostrar:**
- Activación/desactivación de promociones
- Validación de fechas de vigencia
- Comportamiento cuando expira una promoción

### 5. Validaciones y Control de Errores
**Demostrar:**
- Intento de crear promoción sin datos
- Validación de fechas inválidas
- Mensaje de error al seleccionar producto y categoría simultáneamente

### 6. Interfaz Administrativa
**Demostrar:**
- Acceso restringido a administradores
- Lista con filtros y acciones
- Formularios con validación visual
- Confirmaciones de eliminación

## Archivos de Referencia

### Modelos y Entidades
- `VelourEssence.Infraestructure/Models/Promocion.cs` - Modelo principal
- `VelourEssence.Infraestructure/Models/Producto.cs` - Relación con productos
- `VelourEssence.Infraestructure/Models/Categoria.cs` - Relación con categorías

### DTOs y Mapeo
- `VelourEssence.Application/DTOs/PromocionDto.cs` - DTO para visualización
- `VelourEssence.Application/DTOs/CrearPromocionDto.cs` - DTO para creación/edición
- `VelourEssence.Application/Profiles/PromocionProfile.cs` - Mapeo AutoMapper
- `VelourEssence.Application/Profiles/ProductoProfile.cs` - Cálculo de precios con descuento

### Servicios y Repositorios
- `VelourEssence.Application/Services/Interfaces/IServicePromocion.cs` - Interfaz del servicio
- `VelourEssence.Application/Services/Implementations/ServicePromocion.cs` - Implementación del servicio
- `VelourEssence.Infraestructure/Repository/Interfaces/IRepositoryPromocion.cs` - Interfaz del repositorio
- `VelourEssence.Infraestructure/Repository/Implementations/RepositoryPromocion.cs` - Implementación del repositorio

### Controladores y Vistas
- `VelourEssence.Web/Controllers/PromocionController.cs` - Controlador principal
- `VelourEssence.Web/Views/Promocion/Index.cshtml` - Lista de promociones
- `VelourEssence.Web/Views/Promocion/Create.cshtml` - Formulario de creación
- `VelourEssence.Web/Views/Promocion/Edit.cshtml` - Formulario de edición
- `VelourEssence.Web/Views/Promocion/Details.cshtml` - Detalles de promoción
- `VelourEssence.Web/Views/Promocion/Delete.cshtml` - Confirmación de eliminación

### Estilos y Assets
- `VelourEssence.Web/wwwroot/css/forms.css` - Estilos globales de formularios
- `VelourEssence.Web/wwwroot/css/promocion-index.css` - Estilos específicos de promociones

### Configuración
- `VelourEssence.Web/Program.cs` - Configuración de servicios (líneas de AutoMapper y DI)
- `VelourEssence.Infraestructure/Data/VelourEssenceContext.cs` - Configuración de Entity Framework

## Notas Adicionales

### Mejoras Implementadas:
- **Corrección de cálculo de descuentos**: Ahora las promociones por categoría se aplican correctamente a los productos
- **Mejoras visuales**: Formularios con estilos corporativos y mejor usabilidad
- **Validaciones robustas**: Control tanto en cliente como en servidor
- **Mapeo optimizado**: AutoMapper configurado para mostrar nombres de productos y categorías

### Consideraciones de Rendimiento:
- Uso de Include() en queries para evitar lazy loading innecesario
- Índices en campos de fecha para búsquedas de promociones activas
- Cacheo de promociones activas (implementación futura recomendada)

### Escalabilidad:
- Arquitectura preparada para múltiples tipos de promociones
- Posibilidad de agregar promociones por usuario o combinadas
- Sistema de prioridades extensible para futuras funcionalidades
