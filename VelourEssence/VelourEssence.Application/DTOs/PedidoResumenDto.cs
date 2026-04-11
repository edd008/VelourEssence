using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelourEssence.Application.DTOs
{
    // DTO para mostrar resumen básico de pedidos
    public record PedidoResumenDto(
        int IdPedido,              // ID único del pedido
        string NombreEstado,       // Estado actual del pedido
        DateTime FechaPedido,      // Fecha de creación del pedido
        decimal Total,             // Monto total del pedido
        string NombreUsuario       // Nombre del cliente que realizó el pedido
    );
}