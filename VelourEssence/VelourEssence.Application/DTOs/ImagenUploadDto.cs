using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelourEssence.Application.DTOs
{
    // DTO para información de archivo de imagen
    public class ImagenUploadDto
    {
            public string NombreArchivo { get; set; } = string.Empty;
            public string TipoContenido { get; set; } = string.Empty;
            public byte[] ContenidoArchivo { get; set; } = Array.Empty<byte>();
        
    }
}
