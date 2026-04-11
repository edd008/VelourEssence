namespace VelourEssence.Application.DTOs
{
    // DTO para transferencia de datos de etiquetas
    public record EtiquetaDto
    {
        // ID único de la etiqueta
        public int IdEtiqueta { get; set; }

        // Nombre descriptivo de la etiqueta
        public string? Nombre { get; set; }
    }
}