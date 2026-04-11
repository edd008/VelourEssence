namespace VelourEssence.Application.DTOs
{
    // DTO para mostrar información de categorías
    public record CategoriaDto
    {
        // Identificador único de la categoría
        public int IdCategoria { get; set; }

        // Nombre de la categoría
        public string Nombre { get; set; } = null!;
    }
}
