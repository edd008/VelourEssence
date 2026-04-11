namespace VelourEssence.Application.DTOs
{
    public class CrearProductoPersonalizadoDto
    {
        public int IdProductoBase { get; set; }
        public int? IdUsuario { get; set; }
        public List<SeleccionPersonalizacionDto> Selecciones { get; set; } = new List<SeleccionPersonalizacionDto>();
    }
}
