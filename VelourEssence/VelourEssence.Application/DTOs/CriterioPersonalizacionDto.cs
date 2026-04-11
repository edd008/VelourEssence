namespace VelourEssence.Application.DTOs
{
    public class CriterioPersonalizacionDto
    {
        public int IdCriterio { get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string TipoDato { get; set; } = "Lista";
        public bool Obligatorio { get; set; } = true;
        public bool Activo { get; set; } = true;
        public int Orden { get; set; } = 0;
        public List<OpcionPersonalizacionDto> Opciones { get; set; } = new();
    }
}
