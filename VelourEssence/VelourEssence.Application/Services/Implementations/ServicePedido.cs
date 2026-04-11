using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Infraestructure.Repository.Interfaces;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.Services.Implementations
{
    public class ServicePedido : IPedidoService
    {
        private readonly IRepositoryPedido _repositoryPedido;
        private readonly IRepositoryCarrito _repositoryCarrito;
        private readonly IRepositoryProductos _repositoryProductos;
        private readonly IMapper _mapper;

        public ServicePedido(IRepositoryPedido repositoryPedido, IRepositoryCarrito repositoryCarrito, 
                           IRepositoryProductos repositoryProductos, IMapper mapper)
        {
            _repositoryPedido = repositoryPedido;
            _repositoryCarrito = repositoryCarrito;
            _repositoryProductos = repositoryProductos;
            _mapper = mapper;
        }

        public async Task<List<PedidoResumenDto>> ObtenerTodosAsync()
        {
            var pedidos = await _repositoryPedido.ObtenerTodosAsync();
            return _mapper.Map<List<PedidoResumenDto>>(pedidos);
        }

        public async Task<PedidoDetalleDto?> ObtenerPorIdAsync(int id)
        {
            var pedido = await _repositoryPedido.ObtenerPorIdAsync(id);
            if (pedido == null) return null;

            return _mapper.Map<PedidoDetalleDto>(pedido);
        }

        public async Task<List<PedidoResumenDto>> ObtenerPorUsuarioAsync(int idUsuario)
        {
            var pedidos = await _repositoryPedido.ObtenerPorUsuarioAsync(idUsuario);
            return _mapper.Map<List<PedidoResumenDto>>(pedidos);
        }

        public async Task<bool> ActualizarEstadoAsync(int idPedido, int nuevoEstado)
        {
            try
            {
                var pedido = await _repositoryPedido.ObtenerPorIdAsync(idPedido);
                if (pedido == null) return false;

                pedido.IdEstadoPedido = nuevoEstado;
                await _repositoryPedido.ActualizarAsync(pedido);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> CrearPedidoDesdeCarritoAsync(int idCarrito, int idUsuario, string direccionEnvio = "", int estadoPedido = 1)
        {
            try
            {
                Console.WriteLine($"🛒 [SERVICE PEDIDO] Creando pedido desde carrito {idCarrito} para usuario {idUsuario}");

                // Obtener los productos normales del carrito
                var productosCarrito = await _repositoryCarrito.GetProductosCarritoAsync(idCarrito);
                
                // Obtener los productos personalizados del carrito
                var productosPersonalizadosCarrito = await _repositoryCarrito.GetProductosPersonalizadosCarritoAsync(idCarrito);

                // Verificar que el carrito no esté vacío
                if (!productosCarrito.Any() && !productosPersonalizadosCarrito.Any())
                {
                    throw new InvalidOperationException("El carrito está vacío");
                }

                Console.WriteLine($"📦 [SERVICE PEDIDO] Productos normales: {productosCarrito.Count()}, Personalizados: {productosPersonalizadosCarrito.Count()}");

                // Calcular totales incluyendo productos normales y personalizados
                decimal subtotal = 0;

                // Sumar productos normales
                foreach (var item in productosCarrito)
                {
                    var producto = await _repositoryCarrito.GetProductoByIdAsync(item.IdProducto);
                    if (producto != null)
                    {
                        var precioConPromocion = producto.Precio ?? 0;
                        
                        // Verificar si tiene promoción activa
                        var promocion = await _repositoryProductos.GetPromocionActivaAsync(item.IdProducto);
                        if (promocion != null)
                        {
                            precioConPromocion = (producto.Precio ?? 0) * (1 - (promocion.PorcentajeDescuento ?? 0) / 100);
                        }

                        subtotal += (item.Cantidad ?? 0) * precioConPromocion;
                        Console.WriteLine($"💰 [SERVICE PEDIDO] Producto {producto.Nombre}: {item.Cantidad} x ₡{precioConPromocion:F2} = ₡{(item.Cantidad ?? 0) * precioConPromocion:F2}");
                    }
                }

                // Sumar productos personalizados
                foreach (var itemPersonalizado in productosPersonalizadosCarrito)
                {
                    var productoPersonalizado = await _repositoryCarrito.GetProductoPersonalizadoByIdAsync(itemPersonalizado.IdProductoPersonalizado);
                    if (productoPersonalizado != null)
                    {
                        var precioPersonalizado = productoPersonalizado.PrecioFinal;
                        subtotal += (itemPersonalizado.Cantidad ?? 0) * precioPersonalizado;
                        Console.WriteLine($"🎨 [SERVICE PEDIDO] Producto personalizado: {itemPersonalizado.Cantidad} x ₡{precioPersonalizado:F2} = ₡{(itemPersonalizado.Cantidad ?? 0) * precioPersonalizado:F2}");
                    }
                }

                decimal impuesto = subtotal * 0.13m; // 13% de impuesto
                decimal total = subtotal + impuesto;

                Console.WriteLine($"🧾 [SERVICE PEDIDO] Subtotal: ₡{subtotal:F2}, Impuesto: ₡{impuesto:F2}, Total: ₡{total:F2}");

                // Crear el pedido
                var pedido = new Pedido
                {
                    IdUsuario = idUsuario,
                    FechaPedido = DateTime.Now,
                    DireccionEnvio = direccionEnvio,
                    MetodoPago = "Efectivo", // Se establecerá según el método de pago
                    Subtotal = subtotal,
                    Impuesto = impuesto,
                    Total = total,
                    IdEstadoPedido = estadoPedido // Estado recibido como parámetro (1=Pendiente, 2=Pagado)
                };

                // Guardar el pedido
                var pedidoCreado = await _repositoryPedido.CrearAsync(pedido);
                Console.WriteLine($"✅ [SERVICE PEDIDO] Pedido creado con ID: {pedidoCreado.IdPedido}");

                // Crear los detalles del pedido - PRODUCTOS NORMALES
                foreach (var item in productosCarrito)
                {
                    var producto = await _repositoryCarrito.GetProductoByIdAsync(item.IdProducto);
                    if (producto != null)
                    {
                        var precioConPromocion = producto.Precio ?? 0;
                        
                        // Aplicar promoción si existe
                        var promocion = await _repositoryProductos.GetPromocionActivaAsync(item.IdProducto);
                        if (promocion != null)
                        {
                            precioConPromocion = (producto.Precio ?? 0) * (1 - (promocion.PorcentajeDescuento ?? 0) / 100);
                        }

                        var pedidoProducto = new PedidoProducto
                        {
                            IdPedido = pedidoCreado.IdPedido,
                            IdProducto = item.IdProducto,
                            Cantidad = item.Cantidad,
                            PrecioUnitario = precioConPromocion
                        };

                        pedidoCreado.PedidoProducto.Add(pedidoProducto);
                        Console.WriteLine($"📝 [SERVICE PEDIDO] Agregado producto normal: {producto.Nombre}");
                    }
                }

                // TODO: PRODUCTOS PERSONALIZADOS
                // Nota: Aquí necesitaríamos una tabla PedidoProductoPersonalizado para manejar 
                // productos personalizados en pedidos, similar a como CarritoProductoPersonalizado 
                // maneja productos personalizados en carritos.
                // Por ahora, documentamos esto para implementación futura.
                
                if (productosPersonalizadosCarrito.Any())
                {
                    Console.WriteLine($"⚠️ [SERVICE PEDIDO] ATENCIÓN: {productosPersonalizadosCarrito.Count()} productos personalizados NO se agregaron al pedido - requiere tabla PedidoProductoPersonalizado");
                }

                // Actualizar el pedido con los productos
                await _repositoryPedido.ActualizarAsync(pedidoCreado);

                // Limpiar el carrito después de crear el pedido
                await _repositoryCarrito.LimpiarCarritoAsync(idCarrito);
                Console.WriteLine($"🧹 [SERVICE PEDIDO] Carrito {idCarrito} limpiado");

                return pedidoCreado.IdPedido;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SERVICE PEDIDO] Error al crear pedido desde carrito: {ex.Message}");
                throw new InvalidOperationException($"Error al crear pedido desde carrito: {ex.Message}", ex);
            }
        }

        // IMPLEMENTACIONES DE LOS NUEVOS MÉTODOS

        public async Task<int> CrearPedidoDirectoAsync(CrearPedidoDto pedidoDto, int idUsuario)
        {
            try
            {
                Console.WriteLine($"🛒 [SERVICE] Creando pedido directo para usuario {idUsuario}");

                // Calcular totales
                var totalesCalculados = await CalcularTotalesPedidoAsync(new CalcularTotalesRequest
                {
                    Productos = pedidoDto.ProductosDelPedido,
                    ProductosPersonalizados = pedidoDto.ProductosPersonalizadosDelPedido
                });

                // Crear el pedido
                var pedido = new Pedido
                {
                    IdUsuario = idUsuario,
                    FechaPedido = pedidoDto.Fecha,
                    DireccionEnvio = pedidoDto.DireccionEnvio,
                    MetodoPago = pedidoDto.MetodoPago,
                    Subtotal = totalesCalculados.Subtotal,
                    Impuesto = totalesCalculados.Impuesto,
                    Total = totalesCalculados.Total,
                    IdEstadoPedido = 1 // Pendiente por defecto
                };

                var pedidoCreado = await _repositoryPedido.CrearAsync(pedido);
                Console.WriteLine($"✅ [SERVICE] Pedido creado con ID: {pedidoCreado.IdPedido}");

                // Agregar productos normales
                foreach (var producto in pedidoDto.ProductosDelPedido)
                {
                    var pedidoProducto = new PedidoProducto
                    {
                        IdPedido = pedidoCreado.IdPedido,
                        IdProducto = producto.IdProducto,
                        Cantidad = producto.Cantidad,
                        PrecioUnitario = producto.PrecioUnitario
                    };
                    pedidoCreado.PedidoProducto.Add(pedidoProducto);
                }

                // Agregar productos personalizados (si existen)
                // TODO: Implementar cuando esté la estructura de PedidoProductoPersonalizado

                await _repositoryPedido.ActualizarAsync(pedidoCreado);
                return pedidoCreado.IdPedido;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SERVICE] Error al crear pedido directo: {ex.Message}");
                throw new InvalidOperationException($"Error al crear pedido: {ex.Message}", ex);
            }
        }

        public async Task<EditarPedidoDto?> ObtenerParaEditarAsync(int idPedido)
        {
            try
            {
                var pedido = await _repositoryPedido.ObtenerPorIdAsync(idPedido);
                if (pedido == null) return null;

                var editarDto = new EditarPedidoDto
                {
                    IdPedido = pedido.IdPedido,
                    Fecha = pedido.FechaPedido ?? DateTime.Now,
                    DireccionEnvio = pedido.DireccionEnvio ?? string.Empty,
                    Estado = pedido.IdEstadoPedidoNavigation?.NombreEstado ?? "Pendiente",
                    MetodoPago = pedido.MetodoPago ?? string.Empty,
                    NombreCliente = pedido.IdUsuarioNavigation?.NombreUsuario ?? "Usuario",
                    Subtotal = pedido.Subtotal ?? 0,
                    Impuesto = pedido.Impuesto ?? 0,
                    Total = pedido.Total ?? 0
                };

                // Convertir productos del pedido
                foreach (var pp in pedido.PedidoProducto)
                {
                    editarDto.ProductosDelPedido.Add(new ProductoPedidoCrearDto
                    {
                        IdProducto = pp.IdProducto,
                        Cantidad = pp.Cantidad ?? 0,
                        PrecioUnitario = pp.PrecioUnitario ?? 0,
                        NombreProducto = pp.IdProductoNavigation?.Nombre ?? "Producto"
                    });
                }

                return editarDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SERVICE] Error al obtener pedido para editar: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ActualizarPedidoAsync(EditarPedidoDto pedidoDto)
        {
            try
            {
                var pedido = await _repositoryPedido.ObtenerPorIdAsync(pedidoDto.IdPedido);
                if (pedido == null) return false;

                // Actualizar datos básicos
                pedido.FechaPedido = pedidoDto.Fecha;
                pedido.DireccionEnvio = pedidoDto.DireccionEnvio;
                pedido.MetodoPago = pedidoDto.MetodoPago;

                // Recalcular totales
                var totales = await CalcularTotalesPedidoAsync(new CalcularTotalesRequest
                {
                    Productos = pedidoDto.ProductosDelPedido,
                    ProductosPersonalizados = pedidoDto.ProductosPersonalizadosDelPedido
                });

                pedido.Subtotal = totales.Subtotal;
                pedido.Impuesto = totales.Impuesto;
                pedido.Total = totales.Total;

                // Limpiar productos existentes y agregar los nuevos
                pedido.PedidoProducto.Clear();
                foreach (var producto in pedidoDto.ProductosDelPedido)
                {
                    pedido.PedidoProducto.Add(new PedidoProducto
                    {
                        IdPedido = pedido.IdPedido,
                        IdProducto = producto.IdProducto,
                        Cantidad = producto.Cantidad,
                        PrecioUnitario = producto.PrecioUnitario
                    });
                }

                await _repositoryPedido.ActualizarAsync(pedido);
                Console.WriteLine($"✅ [SERVICE] Pedido {pedidoDto.IdPedido} actualizado exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SERVICE] Error al actualizar pedido: {ex.Message}");
                return false;
            }
        }

        public async Task<ProductoParaPedidoDto?> ObtenerProductoParaPedidoAsync(int idProducto)
        {
            try
            {
                var producto = await _repositoryProductos.GetByIdAsync(idProducto);
                if (producto == null) return null;

                // Verificar si tiene promoción activa
                var promocion = await _repositoryProductos.GetPromocionActivaAsync(idProducto);
                
                var dto = new ProductoParaPedidoDto
                {
                    IdProducto = producto.IdProducto,
                    Nombre = producto.Nombre ?? "Producto",
                    Precio = producto.Precio ?? 0,
                    EstaDisponible = true, // Asumiendo que si existe está disponible
                    Stock = producto.Stock ?? 0
                };

                if (promocion != null)
                {
                    dto.TienePromocion = true;
                    dto.PrecioConPromocion = (producto.Precio ?? 0) * (1 - (promocion.PorcentajeDescuento ?? 0) / 100);
                    dto.DescripcionPromocion = promocion.Nombre ?? "Promoción activa";
                }
                else
                {
                    dto.PrecioConPromocion = dto.Precio;
                }

                return dto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SERVICE] Error al obtener producto para pedido: {ex.Message}");
                throw;
            }
        }

        public Task<CalcularTotalesResponse> CalcularTotalesPedidoAsync(CalcularTotalesRequest request)
        {
            try
            {
                var response = new CalcularTotalesResponse();
                decimal subtotal = 0;

                // Calcular productos normales
                foreach (var producto in request.Productos)
                {
                    var subtotalLinea = producto.Cantidad * producto.PrecioUnitario;
                    var impuestoLinea = subtotalLinea * 0.13m; // 13% IVA
                    var totalLinea = subtotalLinea + impuestoLinea;

                    subtotal += subtotalLinea;

                    response.LineasCalculadas.Add(new LineaCalculadaDto
                    {
                        TipoProducto = 1, // Normal
                        IdItem = producto.IdProducto,
                        SubtotalLinea = subtotalLinea,
                        ImpuestoLinea = impuestoLinea,
                        TotalLinea = totalLinea
                    });
                }

                // Calcular productos personalizados
                foreach (var personalizado in request.ProductosPersonalizados)
                {
                    var subtotalLinea = personalizado.Cantidad * personalizado.PrecioUnitario;
                    var impuestoLinea = subtotalLinea * 0.13m; // 13% IVA
                    var totalLinea = subtotalLinea + impuestoLinea;

                    subtotal += subtotalLinea;

                    response.LineasCalculadas.Add(new LineaCalculadaDto
                    {
                        TipoProducto = 2, // Personalizado
                        IdItem = personalizado.IdProductoPersonalizado,
                        SubtotalLinea = subtotalLinea,
                        ImpuestoLinea = impuestoLinea,
                        TotalLinea = totalLinea
                    });
                }

                response.Subtotal = subtotal;
                response.Impuesto = subtotal * 0.13m;
                response.Total = response.Subtotal + response.Impuesto;

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SERVICE] Error al calcular totales: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> EliminarLineaDetalleAsync(int idPedido, int tipoProducto, int idItem)
        {
            try
            {
                var pedido = await _repositoryPedido.ObtenerPorIdAsync(idPedido);
                if (pedido == null) return false;

                if (tipoProducto == 1) // Producto normal
                {
                    var productoAEliminar = pedido.PedidoProducto.FirstOrDefault(p => p.IdProducto == idItem);
                    if (productoAEliminar != null)
                    {
                        pedido.PedidoProducto.Remove(productoAEliminar);
                    }
                }
                // TODO: Implementar eliminación de productos personalizados cuando esté la estructura

                await _repositoryPedido.ActualizarAsync(pedido);
                Console.WriteLine($"✅ [SERVICE] Línea de detalle eliminada del pedido {idPedido}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SERVICE] Error al eliminar línea de detalle: {ex.Message}");
                return false;
            }
        }
    }
}