using VelourEssence.Application.DTOs;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;
using VelourEssence.Application.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace VelourEssence.Application.Services.Implementations
{
    public class ServiceProductoPersonalizado : IServiceProductoPersonalizado
    {
        private readonly IRepositoryProductoPersonalizado _repositoryProductoPersonalizado;
        private readonly IRepositoryCriterioPersonalizacion _repositoryCriterio;
        private readonly IRepositoryOpcionPersonalizacion _repositoryOpcion;
        private readonly IRepositoryProductos _repositoryProducto;
        private readonly IMapper _mapper;
        private readonly ILogger<ServiceProductoPersonalizado> _logger;

        public ServiceProductoPersonalizado(
            IRepositoryProductoPersonalizado repositoryProductoPersonalizado,
            IRepositoryCriterioPersonalizacion repositoryCriterio,
            IRepositoryOpcionPersonalizacion repositoryOpcion,
            IRepositoryProductos repositoryProducto,
            IMapper mapper,
            ILogger<ServiceProductoPersonalizado> logger)
        {
            _repositoryProductoPersonalizado = repositoryProductoPersonalizado;
            _repositoryCriterio = repositoryCriterio;
            _repositoryOpcion = repositoryOpcion;
            _repositoryProducto = repositoryProducto;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CalculoPrecioPersonalizadoDto> CalcularPrecioAsync(CalculoPrecioRequestDto request)
        {
            try
            {
                _logger.LogInformation("Calculando precio para producto {ProductoId} con {Count} selecciones", 
                    request.IdProductoBase, request.Selecciones.Count);

                // Obtener producto base
                var productoBase = await _repositoryProducto.GetByIdAsync(request.IdProductoBase);
                if (productoBase == null)
                {
                    throw new ArgumentException($"Producto con ID {request.IdProductoBase} no encontrado");
                }

                decimal precioBase = productoBase.Precio ?? 0;
                decimal precioPersonalizaciones = 0;

                // Calcular precio de personalizaciones
                foreach (var seleccion in request.Selecciones)
                {
                    var opcion = await _repositoryOpcion.GetAsync(seleccion.IdOpcion);
                    if (opcion == null)
                    {
                        _logger.LogWarning("Opción {OpcionId} no encontrada", seleccion.IdOpcion);
                        continue;
                    }

                    precioPersonalizaciones += opcion.PrecioAdicional;
                    _logger.LogDebug("Agregando precio adicional {Precio} de opción {Opcion}", 
                        opcion.PrecioAdicional, opcion.Etiqueta ?? opcion.Valor);
                }

                var resultado = new CalculoPrecioPersonalizadoDto
                {
                    PrecioBase = precioBase,
                    PrecioPersonalizaciones = precioPersonalizaciones,
                    PrecioTotal = precioBase + precioPersonalizaciones,
                    Selecciones = request.Selecciones
                };

                _logger.LogInformation("Precio calculado: Base={PrecioBase}, Personalizaciones={Personalizaciones}, Total={Total}", 
                    resultado.PrecioBase, resultado.PrecioPersonalizaciones, resultado.PrecioTotal);

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculando precio para producto {ProductoId}", request.IdProductoBase);
                throw;
            }
        }

        public async Task<ValidacionPersonalizacionDto> ValidarSeleccionesAsync(int idProductoBase, List<SeleccionPersonalizacionDto> selecciones)
        {
            try
            {
                var resultado = new ValidacionPersonalizacionDto
                {
                    EsValido = true,
                    Errores = new List<string>()
                };

                // Obtener criterios para el producto
                var criterios = await _repositoryCriterio.GetCriteriosPorProductoAsync(idProductoBase);

                // Validar criterios obligatorios
                foreach (var criterio in criterios.Where(c => c.Obligatorio))
                {
                    var seleccionCriterio = selecciones.FirstOrDefault(s => s.IdCriterio == criterio.IdCriterio);
                    if (seleccionCriterio == null)
                    {
                        resultado.EsValido = false;
                        resultado.Errores.Add($"El criterio '{criterio.Nombre}' es obligatorio");
                    }
                }

                // Validar que las opciones seleccionadas existan y estén activas
                foreach (var seleccion in selecciones)
                {
                    var opcion = await _repositoryOpcion.GetAsync(seleccion.IdOpcion);
                    if (opcion == null)
                    {
                        resultado.EsValido = false;
                        resultado.Errores.Add($"La opción seleccionada no existe");
                        continue;
                    }

                    if (!opcion.Activo)
                    {
                        resultado.EsValido = false;
                        resultado.Errores.Add($"La opción '{opcion.Etiqueta ?? opcion.Valor}' no está disponible");
                    }

                    // Validar que la opción pertenece al criterio correcto
                    if (opcion.IdCriterio != seleccion.IdCriterio)
                    {
                        resultado.EsValido = false;
                        resultado.Errores.Add($"La opción '{opcion.Etiqueta ?? opcion.Valor}' no corresponde al criterio seleccionado");
                    }
                }

                _logger.LogInformation("Validación completada: {EsValido}, Errores: {NumErrores}", 
                    resultado.EsValido, resultado.Errores.Count);

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando selecciones para producto {ProductoId}", idProductoBase);
                throw;
            }
        }

        public async Task<ProductoPersonalizadoDto> CrearProductoPersonalizadoAsync(CrearProductoPersonalizadoDto dto)
        {
            try
            {
                _logger.LogInformation("Creando producto personalizado para usuario {UsuarioId}", dto.IdUsuario);

                // Validar selecciones
                var validacion = await ValidarSeleccionesAsync(dto.IdProductoBase, dto.Selecciones);
                if (!validacion.EsValido)
                {
                    throw new ArgumentException($"Selecciones inválidas: {string.Join(", ", validacion.Errores)}");
                }

                // Calcular precio
                var calculoPrecio = await CalcularPrecioAsync(new CalculoPrecioRequestDto
                {
                    IdProductoBase = dto.IdProductoBase,
                    Selecciones = dto.Selecciones
                });

                // Crear producto personalizado
                var productoPersonalizado = new ProductoPersonalizado
                {
                    IdProductoBase = dto.IdProductoBase,
                    IdUsuario = dto.IdUsuario,
                    PrecioFinal = calculoPrecio.PrecioTotal,
                    FechaCreacion = DateTime.UtcNow,
                    Activo = true
                };

                var productoCreado = await _repositoryProductoPersonalizado.CreateAsync(productoPersonalizado);

                // Crear detalles de personalización
                var detalles = new List<ProductoPersonalizadoDetalle>();
                foreach (var seleccion in dto.Selecciones)
                {
                    var detalle = new ProductoPersonalizadoDetalle
                    {
                        IdProductoPersonalizado = productoCreado.IdProductoPersonalizado,
                        IdCriterio = seleccion.IdCriterio,
                        IdOpcion = seleccion.IdOpcion
                    };
                    detalles.Add(detalle);
                }

                await _repositoryProductoPersonalizado.CrearDetallesAsync(detalles);

                _logger.LogInformation("Producto personalizado creado con ID {ProductoId}", 
                    productoCreado.IdProductoPersonalizado);

                return _mapper.Map<ProductoPersonalizadoDto>(productoCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando producto personalizado");
                throw;
            }
        }

        public async Task<ProductoPersonalizadoDto?> GetProductoPersonalizadoAsync(int id)
        {
            try
            {
                var producto = await _repositoryProductoPersonalizado.GetConDetallesAsync(id);
                return producto != null ? _mapper.Map<ProductoPersonalizadoDto>(producto) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo producto personalizado {ProductoId}", id);
                throw;
            }
        }

        public async Task<List<ProductoPersonalizadoDto>> GetProductosPorUsuarioAsync(int idUsuario)
        {
            try
            {
                var productos = await _repositoryProductoPersonalizado.GetPorUsuarioAsync(idUsuario);
                return _mapper.Map<List<ProductoPersonalizadoDto>>(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos personalizados para usuario {UsuarioId}", idUsuario);
                throw;
            }
        }

        public async Task<List<CriterioPersonalizacionDto>> GetCriteriosPorProductoAsync(int idProducto)
        {
            try
            {
                var criterios = await _repositoryCriterio.GetCriteriosPorProductoAsync(idProducto);
                return _mapper.Map<List<CriterioPersonalizacionDto>>(criterios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo criterios para producto {ProductoId}", idProducto);
                throw;
            }
        }

        public async Task<bool> EliminarProductoPersonalizadoAsync(int id)
        {
            try
            {
                var producto = await _repositoryProductoPersonalizado.GetAsync(id);
                if (producto == null) return false;

                // Soft delete
                producto.Activo = false;
                await _repositoryProductoPersonalizado.UpdateAsync(producto);

                _logger.LogInformation("Producto personalizado {ProductoId} eliminado (soft delete)", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando producto personalizado {ProductoId}", id);
                throw;
            }
        }
    }
}
