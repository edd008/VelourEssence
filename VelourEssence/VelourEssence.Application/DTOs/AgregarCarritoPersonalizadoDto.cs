namespace VelourEssence.Application.DTOs
{
    public class AgregarCarritoPersonalizadoDto
    {
        public int IdProductoBase { get; set; }
        public List<SeleccionPersonalizacionDto> Selecciones { get; set; } = new List<SeleccionPersonalizacionDto>();
        public int Cantidad { get; set; } = 1;
    }
}
