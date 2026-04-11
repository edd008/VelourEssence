namespace VelourEssence.Application.DTOs
{
    public class OpcionPersonalizacionDto
    {
        public int IdOpcion { get; set; }
        public int IdCriterio { get; set; }
        public string Nombre { get; set; } = null!;
        public string Valor { get; set; } = null!;
        public string? Etiqueta { get; set; }
        public decimal PrecioAdicional { get; set; } = 0;
        public bool Activo { get; set; } = true;
        public int Orden { get; set; } = 0;
    }
}
