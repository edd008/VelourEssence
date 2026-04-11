using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelourEssence.Application.DTOs
{
    // DTO para mostrar detalles completos de un pedido
    public record PedidoDetalleDto(
        int IdPedido,
        string? Estado,
        DateTime? Fecha,
        string? DireccionEnvio,
        string? MetodoPago,
        List<ProductoPedidoDto> Productos,
        decimal? Subtotal,
        decimal? Impuesto,
        decimal? Total,
        string? NombreCliente
    )
    {
        // Lista de productos del pedido, inicializada vacía si es null
        public List<ProductoPedidoDto> Productos { get; init; } = Productos ?? new();
    }

    // DTO para productos dentro de un pedido
    public record ProductoPedidoDto(
        string? Nombre,
        int? Cantidad,
        decimal? PrecioUnitario
    )
    {
        // Calcula el precio total del producto (cantidad x precio unitario)
        public decimal? PrecioTotal => Cantidad * PrecioUnitario;
    }
}