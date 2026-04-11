namespace VelourEssence.Application.DTOs
{
    // DTO para el mantenimiento de productos que incluye las opciones disponibles
    public class ProductoMantenimientoDto
    {
        // Listas para los dropdowns
        public List<CategoriaDto> Categorias { get; set; } = new();
        public List<EtiquetaDto> Etiquetas { get; set; } = new();
        
        // Para crear producto
        public CrearProductoDto? CrearProducto { get; set; }
        
        // Para editar producto
        public EditarProductoDto? EditarProducto { get; set; }
        
        // Para mostrar el promedio de valoraciones (solo lectura)
        public double PromedioValoracion { get; set; }
        public int TotalReseñas { get; set; }
    }
}
