using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelourEssence.Application.DTOs;

namespace VelourEssence.Application.Services.Interfaces
{
    // Interfaz para servicios de gestión de pedidos
    public interface IPedidoService
    {
        // Obtiene todos los pedidos en formato resumen
        Task<List<PedidoResumenDto>> ObtenerTodosAsync();

        // Busca un pedido por ID con detalles completos
        Task<PedidoDetalleDto?> ObtenerPorIdAsync(int id);

        // Obtiene pedidos de un usuario específico en formato resumen
        Task<List<PedidoResumenDto>> ObtenerPorUsuarioAsync(int idUsuario);

        // Actualiza el estado de un pedido
        Task<bool> ActualizarEstadoAsync(int idPedido, int nuevoEstado);

        // Crea un pedido desde un carrito
        Task<int> CrearPedidoDesdeCarritoAsync(int idCarrito, int idUsuario, string direccionEnvio = "", int estadoPedido = 1);

        // NUEVOS MÉTODOS PARA GESTIÓN COMPLETA DE PEDIDOS

        // Crea un pedido directamente (no desde carrito)
        Task<int> CrearPedidoDirectoAsync(CrearPedidoDto pedidoDto, int idUsuario);

        // Obtiene un pedido para editar
        Task<EditarPedidoDto?> ObtenerParaEditarAsync(int idPedido);

        // Actualiza un pedido completo
        Task<bool> ActualizarPedidoAsync(EditarPedidoDto pedidoDto);

        // Obtiene información de un producto para agregar al pedido
        Task<ProductoParaPedidoDto?> ObtenerProductoParaPedidoAsync(int idProducto);

        // Calcula los totales de un pedido dinámicamente
        Task<CalcularTotalesResponse> CalcularTotalesPedidoAsync(CalcularTotalesRequest request);

        // Elimina una línea de detalle del pedido
        Task<bool> EliminarLineaDetalleAsync(int idPedido, int tipoProducto, int idItem);
    }
}