using Microsoft.EntityFrameworkCore;
using VelourEssence.Infraestructure.Data;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Infraestructure.Repository.Implementations
{
    public class RepositoryPago : IRepositoryPago
    {
        private readonly VelourEssenceContext _context;

        public RepositoryPago(VelourEssenceContext context)
        {
            _context = context;
        }

        public async Task<Pago> CreateAsync(Pago pago)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO PAGO] Creando pago: IdPedido={pago.IdPedido}, TipoPago={pago.TipoPago}, Monto={pago.MontoTotal}");
                _context.Pago.Add(pago);
                await _context.SaveChangesAsync();
                Console.WriteLine($"[DEBUG REPO PAGO] Pago creado exitosamente con ID: {pago.IdPago}");
                return pago;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO PAGO] Error al crear el pago: {ex.Message}");
                Console.WriteLine($"[ERROR REPO PAGO] StackTrace: {ex.StackTrace}");
                throw new Exception($"Error al crear el pago: {ex.Message}", ex);
            }
        }

        public async Task<Pago?> GetByIdAsync(int idPago)
        {
            try
            {
                return await _context.Pago
                    .Include(p => p.IdPedidoNavigation)
                    .FirstOrDefaultAsync(p => p.IdPago == idPago);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el pago: {ex.Message}", ex);
            }
        }

        public async Task<List<Pago>> GetByPedidoIdAsync(int idPedido)
        {
            try
            {
                return await _context.Pago
                    .Where(p => p.IdPedido == idPedido)
                    .OrderByDescending(p => p.FechaPago)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los pagos del pedido: {ex.Message}", ex);
            }
        }

        public async Task<List<Pago>> GetByUsuarioIdAsync(int idUsuario)
        {
            try
            {
                Console.WriteLine($"[DEBUG REPO] Buscando pagos para usuario {idUsuario}");
                
                // Primero verificar cuántos pagos hay en total
                var totalPagos = await _context.Pago.CountAsync();
                Console.WriteLine($"[DEBUG REPO] Total de pagos en la base de datos: {totalPagos}");
                
                // Verificar cuántos pedidos hay
                var totalPedidos = await _context.Pedido.CountAsync();
                Console.WriteLine($"[DEBUG REPO] Total de pedidos en la base de datos: {totalPedidos}");
                
                // Verificar pedidos del usuario
                var pedidosUsuario = await _context.Pedido.Where(p => p.IdUsuario == idUsuario).ToListAsync();
                Console.WriteLine($"[DEBUG REPO] Pedidos del usuario {idUsuario}: {pedidosUsuario.Count}");
                
                foreach (var pedido in pedidosUsuario)
                {
                    Console.WriteLine($"[DEBUG REPO] Pedido ID: {pedido.IdPedido}, Estado: {pedido.IdEstadoPedido}, Fecha: {pedido.FechaPedido}");
                }
                
                // Verificar si hay pagos para estos pedidos
                var pedidoIds = pedidosUsuario.Select(p => p.IdPedido).ToList();
                var pagosParaPedidos = await _context.Pago.Where(p => pedidoIds.Contains(p.IdPedido)).ToListAsync();
                Console.WriteLine($"[DEBUG REPO] Pagos para pedidos del usuario: {pagosParaPedidos.Count}");
                
                foreach (var pago in pagosParaPedidos)
                {
                    Console.WriteLine($"[DEBUG REPO] Pago ID: {pago.IdPago}, Pedido ID: {pago.IdPedido}, Monto: {pago.MontoTotal}");
                }
                
                // Usar consulta sin filtro de estado por ahora para debugging
                var result = await _context.Pago
                    .Include(p => p.IdPedidoNavigation)
                        .ThenInclude(pedido => pedido.PedidoProducto)
                    .Where(p => p.IdPedidoNavigation.IdUsuario == idUsuario)
                    .OrderByDescending(p => p.FechaPago)
                    .ToListAsync();
                
                Console.WriteLine($"[DEBUG REPO] Resultados finales encontrados: {result.Count}");
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR REPO] Error al obtener los pagos del usuario: {ex.Message}");
                Console.WriteLine($"[ERROR REPO] StackTrace: {ex.StackTrace}");
                throw new Exception($"Error al obtener los pagos del usuario: {ex.Message}", ex);
            }
        }

        public async Task<Pago> UpdateAsync(Pago pago)
        {
            try
            {
                _context.Pago.Update(pago);
                await _context.SaveChangesAsync();
                return pago;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el pago: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int idPago)
        {
            try
            {
                var pago = await GetByIdAsync(idPago);
                if (pago == null)
                    return false;

                _context.Pago.Remove(pago);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar el pago: {ex.Message}", ex);
            }
        }
    }
}
