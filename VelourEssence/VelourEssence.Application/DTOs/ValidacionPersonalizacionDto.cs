namespace VelourEssence.Application.DTOs
{
    public class ValidacionPersonalizacionDto
    {
        public bool EsValido { get; set; }
        public List<string> Errores { get; set; } = new List<string>();
    }
}
