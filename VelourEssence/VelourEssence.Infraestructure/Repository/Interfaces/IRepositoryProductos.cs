using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Infraestructure.Repository.Interfaces
{
    // Interfaz para operaciones de repositorio de productos
    public interface IRepositoryProductos
    {
        // Obtiene todos los productos disponibles
        Task<ICollection<Producto>> ListAsync();

        // Busca un producto por ID, retorna null si no existe
        Task<Producto?> GetByIdAsync(int id);

        // Crea un nuevo producto
        Task<Producto> CreateAsync(Producto producto);

        // Actualiza un producto existente
        Task<Producto> UpdateAsync(Producto producto);

        // Elimina un producto por ID
        Task<bool> DeleteAsync(Producto producto);

            Task EliminarImagenAsync(ImagenProducto imagen);




        // Verifica si existe un producto con el nombre especificado (para validación)
        Task<bool> ExistsAsync(string nombre, int? excludeId = null);

        // Obtiene las categorías disponibles
        Task<ICollection<Categoria>> GetCategoriasAsync();

        // Obtiene las etiquetas disponibles
        Task<ICollection<Etiqueta>> GetEtiquetasAsync();

        // Obtiene la promoción activa para un producto específico
        Task<Promocion?> GetPromocionActivaAsync(int idProducto);

        // Guarda los cambios en la base de datos
        Task SaveChangesAsync();
    }
}