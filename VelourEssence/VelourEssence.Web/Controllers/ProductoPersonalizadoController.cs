using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Application.DTOs;
using System.Security.Claims;

namespace VelourEssence.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoPersonalizadoController : ControllerBase
    {
        private readonly IServiceProductoPersonalizado _serviceProductoPersonalizado;
        private readonly IPersonalizacionService _personalizacionService;
        private readonly ILogger<ProductoPersonalizadoController> _logger;

        public ProductoPersonalizadoController(
            IServiceProductoPersonalizado serviceProductoPersonalizado,
            IPersonalizacionService personalizacionService,
            ILogger<ProductoPersonalizadoController> logger)
        {
            _serviceProductoPersonalizado = serviceProductoPersonalizado;
            _personalizacionService = personalizacionService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene los criterios de personalización disponibles para un producto
        /// </summary>
        [HttpGet("criterios/{idProducto}")]
        public async Task<ActionResult<List<CriterioPersonalizacionDto>>> GetCriteriosPorProducto(int idProducto)
        {
            try
            {
                var criterios = await _personalizacionService.GetCriteriosPorProductoAsync(idProducto);
                return Ok(criterios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo criterios para producto {ProductoId}", idProducto);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Calcula el precio de un producto personalizado
        /// </summary>
        [HttpPost("calcular-precio")]
        public async Task<ActionResult<CalculoPrecioPersonalizadoDto>> CalcularPrecio([FromBody] CalculoPrecioRequestDto request)
        {
            try
            {
                if (request == null || request.Selecciones == null || !request.Selecciones.Any())
                {
                    return BadRequest(new { message = "Debe proporcionar al menos una selección" });
                }

                var resultado = await _serviceProductoPersonalizado.CalcularPrecioAsync(request);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación calculando precio");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculando precio");
                return StatusCode(500, new { message = "Error calculando el precio" });
            }
        }

        /// <summary>
        /// Valida las selecciones de personalización
        /// </summary>
        [HttpPost("validar/{idProducto}")]
        public async Task<ActionResult<ValidacionPersonalizacionDto>> ValidarSelecciones(
            int idProducto, 
            [FromBody] List<SeleccionPersonalizacionDto> selecciones)
        {
            try
            {
                var resultado = await _serviceProductoPersonalizado.ValidarSeleccionesAsync(idProducto, selecciones);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando selecciones para producto {ProductoId}", idProducto);
                return StatusCode(500, new { message = "Error validando las selecciones" });
            }
        }

        /// <summary>
        /// Crea un producto personalizado y lo agrega al carrito
        /// </summary>
        [HttpPost("agregar-carrito")]
        [Authorize]
        public async Task<ActionResult<ProductoPersonalizadoDto>> AgregarAlCarrito([FromBody] AgregarCarritoPersonalizadoDto request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var crearDto = new CrearProductoPersonalizadoDto
                {
                    IdProductoBase = request.IdProductoBase,
                    IdUsuario = userId,
                    Selecciones = request.Selecciones
                };

                var producto = await _serviceProductoPersonalizado.CrearProductoPersonalizadoAsync(crearDto);

                // TODO: Aquí se podría agregar automáticamente al carrito del usuario
                // Por ahora solo creamos el producto personalizado

                return Ok(new { 
                    message = "Producto personalizado creado exitosamente",
                    producto = producto
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación agregando al carrito");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error agregando producto personalizado al carrito");
                return StatusCode(500, new { message = "Error agregando al carrito" });
            }
        }

        /// <summary>
        /// Obtiene un producto personalizado por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ProductoPersonalizadoDto>> GetProductoPersonalizado(int id)
        {
            try
            {
                var producto = await _serviceProductoPersonalizado.GetProductoPersonalizadoAsync(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto personalizado no encontrado" });
                }

                // Verificar que el usuario tenga acceso al producto
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                if (producto.IdUsuario != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo producto personalizado {ProductoId}", id);
                return StatusCode(500, new { message = "Error obteniendo el producto" });
            }
        }

        /// <summary>
        /// Obtiene todos los productos personalizados de un usuario
        /// </summary>
        [HttpGet("usuario/mis-productos")]
        [Authorize]
        public async Task<ActionResult<List<ProductoPersonalizadoDto>>> GetMisProductosPersonalizados()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var productos = await _serviceProductoPersonalizado.GetProductosPorUsuarioAsync(userId);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos personalizados del usuario");
                return StatusCode(500, new { message = "Error obteniendo los productos" });
            }
        }

        /// <summary>
        /// Elimina un producto personalizado
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> EliminarProductoPersonalizado(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                // Verificar que el producto existe y pertenece al usuario
                var producto = await _serviceProductoPersonalizado.GetProductoPersonalizadoAsync(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto personalizado no encontrado" });
                }

                if (producto.IdUsuario != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var eliminado = await _serviceProductoPersonalizado.EliminarProductoPersonalizadoAsync(id);
                if (eliminado)
                {
                    return Ok(new { message = "Producto personalizado eliminado exitosamente" });
                }
                else
                {
                    return NotFound(new { message = "No se pudo eliminar el producto" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando producto personalizado {ProductoId}", id);
                return StatusCode(500, new { message = "Error eliminando el producto" });
            }
        }
    }
}
