using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Infraestructure.Repository.Interfaces
{
    // Interfaz para operaciones de repositorio de pedidos
    public interface IRepositoryPedido
    {
        // Obtiene todos los pedidos del sistema
        Task<List<Pedido>> ObtenerTodosAsync();

        // Busca un pedido por ID, retorna null si no existe
        Task<Pedido?> ObtenerPorIdAsync(int id);

        // Obtiene todos los pedidos de un usuario específico
        Task<List<Pedido>> ObtenerPorUsuarioAsync(int idUsuario);

        // Crea un nuevo pedido
        Task<Pedido> CrearAsync(Pedido pedido);

        // Actualiza un pedido existente
        Task<Pedido> ActualizarAsync(Pedido pedido);

        // Elimina un pedido por ID
        Task<bool> EliminarAsync(int id);

        // Verifica si existe un pedido con el ID dado
        Task<bool> ExisteAsync(int id);
    }
}