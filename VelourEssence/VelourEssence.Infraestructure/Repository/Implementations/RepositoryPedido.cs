using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VelourEssence.Infraestructure.Data;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Infraestructure.Repository.Implementations
{
    // Repositorio para manejar los pedidos del sistema
    public class RepositoryPedido : IRepositoryPedido
    {
        private readonly VelourEssenceContext _context;

        // Constructor con inyecci�n del contexto de BD
        public RepositoryPedido(VelourEssenceContext context)
        {
            _context = context;
        }

        // Obtiene todos los pedidos con sus relaciones
        public async Task<List<Pedido>> ObtenerTodosAsync()
        {
            return await _context.Pedido
                .Include(p => p.IdEstadoPedidoNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .Include(p => p.PedidoProducto)
                    .ThenInclude(pp => pp.IdProductoNavigation)
                .ToListAsync();
        }

        // Busca un pedido por ID con todas sus relaciones
        public async Task<Pedido?> ObtenerPorIdAsync(int id)
        {
            return await _context.Pedido
                .Include(p => p.IdEstadoPedidoNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .Include(p => p.PedidoProducto)
                    .ThenInclude(pp => pp.IdProductoNavigation)
                .FirstOrDefaultAsync(p => p.IdPedido == id);
        }

        // Obtiene todos los pedidos de un usuario espec�fico
        public async Task<List<Pedido>> ObtenerPorUsuarioAsync(int idUsuario)
        {
            return await _context.Pedido
                .Include(p => p.IdEstadoPedidoNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .Include(p => p.PedidoProducto)
                    .ThenInclude(pp => pp.IdProductoNavigation)
                .Where(p => p.IdUsuario == idUsuario)
                .ToListAsync();
        }

        // Crea un nuevo pedido en la BD
        public async Task<Pedido> CrearAsync(Pedido pedido)
        {
            _context.Pedido.Add(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }

        // Actualiza un pedido existente
        public async Task<Pedido> ActualizarAsync(Pedido pedido)
        {
            _context.Entry(pedido).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return pedido;
        }

        // Elimina un pedido por ID
        public async Task<bool> EliminarAsync(int id)
        {
            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido == null) return false;

            _context.Pedido.Remove(pedido);
            await _context.SaveChangesAsync();
            return true;
        }

        // Verifica si existe un pedido con el ID dado
        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Pedido.AnyAsync(p => p.IdPedido == id);
        }
    }
}