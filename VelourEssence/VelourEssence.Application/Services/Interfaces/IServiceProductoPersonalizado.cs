using VelourEssence.Application.DTOs;

namespace VelourEssence.Application.Services.Interfaces
{
    /// <summary>
    /// Interfaz para servicios de gestión de productos personalizados
    /// </summary>
    public interface IServiceProductoPersonalizado
    {
        // Cálculo dinámico de precios
        Task<CalculoPrecioPersonalizadoDto> CalcularPrecioAsync(CalculoPrecioRequestDto request);
        
        // Validación de selecciones
        Task<ValidacionPersonalizacionDto> ValidarSeleccionesAsync(int idProductoBase, List<SeleccionPersonalizacionDto> selecciones);
        
        // CRUD de productos personalizados
        Task<ProductoPersonalizadoDto> CrearProductoPersonalizadoAsync(CrearProductoPersonalizadoDto dto);
        Task<ProductoPersonalizadoDto?> GetProductoPersonalizadoAsync(int id);
        Task<List<ProductoPersonalizadoDto>> GetProductosPorUsuarioAsync(int idUsuario);
        Task<bool> EliminarProductoPersonalizadoAsync(int id);
        
        // Criterios de personalización
        Task<List<CriterioPersonalizacionDto>> GetCriteriosPorProductoAsync(int idProducto);
    }
}
