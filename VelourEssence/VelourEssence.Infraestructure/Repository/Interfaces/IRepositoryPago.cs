using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPago
    {
        Task<Pago> CreateAsync(Pago pago);
        Task<Pago?> GetByIdAsync(int idPago);
        Task<List<Pago>> GetByPedidoIdAsync(int idPedido);
        Task<List<Pago>> GetByUsuarioIdAsync(int idUsuario);
        Task<Pago> UpdateAsync(Pago pago);
        Task<bool> DeleteAsync(int idPago);
    }
}
