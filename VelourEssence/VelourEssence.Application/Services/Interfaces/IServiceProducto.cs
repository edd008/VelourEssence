using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelourEssence.Application.DTOs;

namespace VelourEssence.Application.Services.Interfaces
{
    // Interfaz para servicios de gestión de productos
    public interface IServiceProducto
    {
        // Obtiene todos los productos en formato básico
        Task<ICollection<ProductoDto>> ListAsync();

        // Busca un producto por ID en formato básico
        Task<ProductoDto?> GetByIdAsync(int id);

        // Obtiene detalles completos de un producto por ID
        Task<ProductoDetalleDto?> GetDetalleByIdAsync(int id);

        // Crea un nuevo producto
        Task<ProductoDto> CreateAsync(CrearProductoDto crearProductoDto);

        // Actualiza un producto existente
        Task<ProductoDto> UpdateAsync(EditarProductoDto editarProductoDto);

        // Elimina un producto por ID
        Task<bool> DeleteAsync(int id);

        // Obtiene los datos necesarios para el mantenimiento (categorías, etiquetas, etc.)
        Task<ProductoMantenimientoDto> GetMantenimientoDatosAsync();

        // Obtiene los datos de un producto para edición
        Task<ProductoMantenimientoDto> GetForEditAsync(int id);

        // Verifica si existe un producto con el nombre especificado
        Task<bool> ExistsAsync(string nombre, int? excludeId = null);
    }
}