namespace VelourEssence.Application.DTOs
{
    public class CalculoPrecioRequestDto
    {
        public int IdProductoBase { get; set; }
        public List<SeleccionPersonalizacionDto> Selecciones { get; set; } = new List<SeleccionPersonalizacionDto>();
    }
}
