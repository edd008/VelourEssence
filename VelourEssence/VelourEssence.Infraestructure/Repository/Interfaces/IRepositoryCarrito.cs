using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryCarrito
    {
        Task<Carrito?> GetByUsuarioIdAsync(int idUsuario);
        Task<Carrito?> GetByIdAsync(int idCarrito);
        Task<Carrito> CreateAsync(Carrito carrito);
        Task<Carrito> UpdateAsync(Carrito carrito);
        Task<bool> DeleteAsync(Carrito carrito);
        Task<CarritoProducto?> GetCarritoProductoAsync(int idCarrito, int idProducto);
        Task<CarritoProducto> AddProductoAsync(CarritoProducto carritoProducto);
        Task<CarritoProducto> UpdateProductoAsync(CarritoProducto carritoProducto);
        Task<bool> RemoveProductoAsync(CarritoProducto carritoProducto);
        Task<CarritoProductoPersonalizado?> GetCarritoProductoPersonalizadoAsync(int idCarrito, int idProductoPersonalizado);
        Task<CarritoProductoPersonalizado> AddProductoPersonalizadoAsync(CarritoProductoPersonalizado carritoProductoPersonalizado);
        Task<CarritoProductoPersonalizado> UpdateProductoPersonalizadoAsync(CarritoProductoPersonalizado carritoProductoPersonalizado);
        Task<bool> RemoveProductoPersonalizadoAsync(CarritoProductoPersonalizado carritoProductoPersonalizado);
        Task<List<CarritoProducto>> GetProductosCarritoAsync(int idCarrito);
        Task<List<CarritoProductoPersonalizado>> GetProductosPersonalizadosCarritoAsync(int idCarrito);
        Task<bool> LimpiarCarritoAsync(int idCarrito);
        Task<Producto?> GetProductoByIdAsync(int idProducto);
        Task<string> GetPrimeraImagenProductoAsync(int idProducto);
        Task<Promocion?> GetPromocionActivaProductoAsync(int idProducto);
        Task<ProductoPersonalizado?> GetProductoPersonalizadoByIdAsync(int idProductoPersonalizado);
    }
}
