using VelourEssence.Application.DTOs;

namespace VelourEssence.Application.Services.Interfaces
{
    public interface IServiceCarrito
    {
        Task<CarritoDto> ObtenerCarritoUsuario(int idUsuario);
        Task<bool> AgregarProducto(int idUsuario, int idProducto, int cantidad);
        Task<bool> ActualizarCantidad(int idUsuario, int idProducto, int cantidad, bool esPersonalizado = false);
        Task<bool> EliminarProducto(int idUsuario, int idProducto, bool esPersonalizado = false);
        Task<bool> LimpiarCarrito(int idUsuario);
        Task<int> ObtenerCantidadTotal(int idUsuario);
        Task<decimal> ObtenerTotal(int idUsuario);
        Task<bool> VerificarStock(int idUsuario);
    }
}
