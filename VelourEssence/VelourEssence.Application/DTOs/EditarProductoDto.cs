using System.ComponentModel.DataAnnotations;

namespace VelourEssence.Application.DTOs
{
    // DTO para editar un producto existente
    public class EditarProductoDto
    {
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La categoría es requerida")]
        public int IdCategoria { get; set; }

        [StringLength(50, ErrorMessage = "La marca no puede exceder los 50 caracteres")]
        public string? Marca { get; set; }

        [StringLength(50, ErrorMessage = "La concentración no puede exceder los 50 caracteres")]
        public string? Concentracion { get; set; }

        [Required(ErrorMessage = "El género es requerido")]
        [StringLength(20, ErrorMessage = "El género no puede exceder los 20 caracteres")]
        public string? Genero { get; set; }


        // Lista de IDs de etiquetas seleccionadas
        public List<int> EtiquetasIds { get; set; } = new();

        // Archivos de imágenes nuevas para cargar
        public List<ImagenUploadDto> ImagenesNuevasArchivos { get; set; } = new();

        // IDs de imágenes existentes que se deben eliminar
        public List<int> ImagenesAEliminar { get; set; } = new();

        // Imágenes existentes del producto (para mostrar en la vista)
        public List<ImagenProductoDto> ImagenesExistentes { get; set; } = new();
    }
}
