namespace VelourEssence.Application.DTOs
{
    public class ResumenCarritoDto
    {
        public int CantidadItems { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
    }
}
