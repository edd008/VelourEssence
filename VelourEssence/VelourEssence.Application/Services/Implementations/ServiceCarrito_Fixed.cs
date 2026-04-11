using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Application.Services.Implementations
{
    public class ServiceCarritoFixed : IServiceCarrito
    {
        private readonly IRepositoryCarrito _repositoryCarrito;
        private readonly IMapper _mapper;
        private const decimal PORCENTAJE_IMPUESTO = 0.13m; // 13% IVA

        public ServiceCarritoFixed(IRepositoryCarrito repositoryCarrito, IMapper mapper)
        {
            _repositoryCarrito = repositoryCarrito;
            _mapper = mapper;
        }

        public async Task<CarritoDto> ObtenerCarritoUsuario(int idUsuario)
        {
            try
            {
                Console.WriteLine($"[DEBUG SERVICE] ObtenerCarritoUsuario para usuario: {idUsuario}");
                
                var carrito = await _repositoryCarrito.GetByUsuarioIdAsync(idUsuario);

                if (carrito == null)
                {
                    Console.WriteLine("[DEBUG SERVICE] No hay carrito, creando uno vacío");
                    return new CarritoDto
                    {
                        IdCarrito = 0,
                        IdUsuario = idUsuario,
                        FechaCreacion = DateTime.Now,
                        Productos = new List<CarritoProductoDto>(),
                        ProductosPersonalizados = new List<CarritoProductoPersonalizadoDto>(),
                        Total = 0.00m,
                        Impuestos = 0.00m,
                        TotalConImpuestos = 0.00m,
                        CantidadTotal = 0
                    };
                }

                Console.WriteLine($"[DEBUG SERVICE] Carrito encontrado ID: {carrito.IdCarrito}");

                // Obtener productos del carrito
                var productosCarrito = await _repositoryCarrito.GetProductosCarritoAsync(carrito.IdCarrito);
                Console.WriteLine($"[DEBUG SERVICE] Productos en carrito: {productosCarrito.Count}");

                var productosDto = new List<CarritoProductoDto>();
                
                foreach (var cp in productosCarrito)
                {
                    Console.WriteLine($"[DEBUG SERVICE] Procesando producto ID: {cp.IdProducto}, Cantidad: {cp.Cantidad}");
                    
                    var producto = await _repositoryCarrito.GetProductoByIdAsync(cp.IdProducto);
                    
                    if (producto != null)
                    {
                        var precioOriginal = producto.Precio ?? 100.00m;
                        var cantidad = cp.Cantidad ?? 0;
                        var precioFinal = precioOriginal;
                        var promocionAplicada = "";
                        
                        // Verificar promoción activa
                        var promocion = await _repositoryCarrito.GetPromocionActivaProductoAsync(cp.IdProducto);
                        if (promocion != null && promocion.PorcentajeDescuento.HasValue)
                        {
                            var descuentoPromocion = promocion.PorcentajeDescuento.Value;
                            precioFinal = precioOriginal * (1 - (descuentoPromocion / 100));
                            promocionAplicada = $"{promocion.Nombre} (-{descuentoPromocion}%)";
                            Console.WriteLine($"[DEBUG SERVICE] Promoción aplicada: {promocionAplicada}");
                        }
                        
                        var productoDto = new CarritoProductoDto
                        {
                            IdProducto = cp.IdProducto,
                            NombreProducto = producto.Nombre ?? $"Producto {cp.IdProducto}",
                            PrecioUnitario = precioFinal,
                            Cantidad = cantidad,
                            Subtotal = cantidad * precioFinal,
                            ImagenUrl = await _repositoryCarrito.GetPrimeraImagenProductoAsync(cp.IdProducto),
                            Stock = producto.Stock ?? 100
                        };
                        
                        if (!string.IsNullOrEmpty(promocionAplicada))
                        {
                            productoDto.NombreProducto += $" - {promocionAplicada}";
                        }
                        
                        productosDto.Add(productoDto);
                        Console.WriteLine($"[DEBUG SERVICE] Producto agregado: {productoDto.NombreProducto}, Precio: {productoDto.PrecioUnitario}");
                    }
                }

                // Obtener productos personalizados
                var productosPersonalizados = await _repositoryCarrito.GetProductosPersonalizadosCarritoAsync(carrito.IdCarrito);
                Console.WriteLine($"[DEBUG SERVICE] Productos personalizados: {productosPersonalizados.Count}");

                var productosPersonalizadosDto = new List<CarritoProductoPersonalizadoDto>();
                
                foreach (var cpp in productosPersonalizados)
                {
                    Console.WriteLine($"[DEBUG SERVICE] Procesando producto personalizado ID: {cpp.IdProductoPersonalizado}");
                    
                    var productoPersonalizado = await _repositoryCarrito.GetProductoPersonalizadoByIdAsync(cpp.IdProductoPersonalizado);
                    
                    if (productoPersonalizado != null)
                    {
                        var cantidad = cpp.Cantidad ?? 0;
                        var precioFinal = productoPersonalizado.PrecioFinal;
                        
                        var personalizadoDto = new CarritoProductoPersonalizadoDto
                        {
                            IdProductoBase = productoPersonalizado.IdProductoBase,
                            NombreProductoBase = $"{productoPersonalizado.BaseProducto?.Nombre ?? "Producto Personalizado"} - {productoPersonalizado.Descripcion}",
                            PrecioBase = precioFinal,
                            Cantidad = cantidad,
                            Subtotal = cantidad * precioFinal,
                            Personalizaciones = new List<PersonalizacionSeleccionadaDto>(),
                            TotalPersonalizacion = 0.00m
                        };
                        
                        productosPersonalizadosDto.Add(personalizadoDto);
                        Console.WriteLine($"[DEBUG SERVICE] Producto personalizado agregado: {personalizadoDto.NombreProductoBase}");
                    }
                }

                var carritoDto = new CarritoDto
                {
                    IdCarrito = carrito.IdCarrito,
                    IdUsuario = carrito.IdUsuario ?? 0,
                    FechaCreacion = carrito.FechaCreacion ?? DateTime.Now,
                    Productos = productosDto,
                    ProductosPersonalizados = productosPersonalizadosDto
                };

                // Calcular totales
                var subtotalProductos = productosDto.Sum(p => p.Subtotal);
                var subtotalPersonalizados = productosPersonalizadosDto.Sum(p => p.Subtotal);
                
                carritoDto.Total = subtotalProductos + subtotalPersonalizados;
                carritoDto.Impuestos = Math.Round(carritoDto.Total * PORCENTAJE_IMPUESTO, 2);
                carritoDto.TotalConImpuestos = carritoDto.Total + carritoDto.Impuestos;
                carritoDto.CantidadTotal = productosDto.Sum(p => p.Cantidad) + productosPersonalizadosDto.Sum(p => p.Cantidad);

                Console.WriteLine($"[DEBUG SERVICE] Carrito final - Productos: {carritoDto.Productos.Count}");
                Console.WriteLine($"[DEBUG SERVICE] Subtotal: {carritoDto.Total}");
                Console.WriteLine($"[DEBUG SERVICE] Impuestos (13%): {carritoDto.Impuestos}");
                Console.WriteLine($"[DEBUG SERVICE] Total: {carritoDto.TotalConImpuestos}");

                return carritoDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error en ObtenerCarritoUsuario: {ex.Message}");
                return new CarritoDto
                {
                    IdCarrito = 0,
                    IdUsuario = idUsuario,
                    FechaCreacion = DateTime.Now,
                    Productos = new List<CarritoProductoDto>(),
                    ProductosPersonalizados = new List<CarritoProductoPersonalizadoDto>(),
                    Total = 0.00m,
                    Impuestos = 0.00m,
                    TotalConImpuestos = 0.00m,
                    CantidadTotal = 0
                };
            }
        }

        public async Task<bool> AgregarProducto(int idUsuario, int idProducto, int cantidad)
        {
            try
            {
                Console.WriteLine($"[DEBUG SERVICE] AgregarProducto - Usuario: {idUsuario}, Producto: {idProducto}, Cantidad: {cantidad}");
                
                Carrito? carrito = null;
                
                try
                {
                    Console.WriteLine("[DEBUG SERVICE] Paso 1: Obteniendo carrito existente");
                    carrito = await _repositoryCarrito.GetByUsuarioIdAsync(idUsuario);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR SERVICE] Error obteniendo carrito: {ex.Message}");
                }

                if (carrito == null)
                {
                    try
                    {
                        Console.WriteLine("[DEBUG SERVICE] Paso 2: Creando nuevo carrito");
                        carrito = new Carrito
                        {
                            IdUsuario = idUsuario,
                            FechaCreacion = DateTime.Now,
                            Activo = true
                        };
                        carrito = await _repositoryCarrito.CreateAsync(carrito);
                        Console.WriteLine($"[DEBUG SERVICE] Carrito creado con ID: {carrito.IdCarrito}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR SERVICE] Error creando carrito: {ex.Message}");
                        return false;
                    }
                }

                try
                {
                    Console.WriteLine("[DEBUG SERVICE] Paso 3: Verificando producto existente en carrito");
                    var itemExistente = await _repositoryCarrito.GetCarritoProductoAsync(carrito.IdCarrito, idProducto);
                    
                    if (itemExistente != null)
                    {
                        Console.WriteLine("[DEBUG SERVICE] Paso 4a: Actualizando cantidad de producto existente");
                        itemExistente.Cantidad = (itemExistente.Cantidad ?? 0) + cantidad;
                        await _repositoryCarrito.UpdateProductoAsync(itemExistente);
                        Console.WriteLine($"[DEBUG SERVICE] Cantidad actualizada a: {itemExistente.Cantidad}");
                    }
                    else
                    {
                        Console.WriteLine("[DEBUG SERVICE] Paso 4b: Agregando nuevo producto al carrito");
                        var nuevoItem = new CarritoProducto
                        {
                            IdCarrito = carrito.IdCarrito,
                            IdProducto = idProducto,
                            Cantidad = cantidad
                        };
                        await _repositoryCarrito.AddProductoAsync(nuevoItem);
                        Console.WriteLine("[DEBUG SERVICE] Producto agregado exitosamente");
                    }
                    
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR SERVICE] Error en operación de producto: {ex.Message}");
                    Console.WriteLine($"[ERROR SERVICE] Stack: {ex.StackTrace}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error general en AgregarProducto: {ex.Message}");
                Console.WriteLine($"[ERROR SERVICE] Stack: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> AgregarProductoPersonalizado(int idUsuario, int idProductoPersonalizado, int cantidad)
        {
            try
            {
                var carrito = await _repositoryCarrito.GetByUsuarioIdAsync(idUsuario);
                
                if (carrito == null)
                {
                    carrito = new Carrito
                    {
                        IdUsuario = idUsuario,
                        FechaCreacion = DateTime.Now,
                        Activo = true
                    };
                    carrito = await _repositoryCarrito.CreateAsync(carrito);
                }

                var nuevoItem = new CarritoProductoPersonalizado
                {
                    IdCarrito = carrito.IdCarrito,
                    IdProductoPersonalizado = idProductoPersonalizado,
                    Cantidad = cantidad
                };

                await _repositoryCarrito.AddProductoPersonalizadoAsync(nuevoItem);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error en AgregarProductoPersonalizado: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarCantidad(int idUsuario, int idProducto, int nuevaCantidad, bool esPersonalizado = false)
        {
            try
            {
                var carrito = await _repositoryCarrito.GetByUsuarioIdAsync(idUsuario);
                if (carrito == null) return false;

                if (!esPersonalizado)
                {
                    var item = await _repositoryCarrito.GetCarritoProductoAsync(carrito.IdCarrito, idProducto);
                    if (item != null)
                    {
                        item.Cantidad = nuevaCantidad;
                        await _repositoryCarrito.UpdateProductoAsync(item);
                        return true;
                    }
                }
                else
                {
                    var item = await _repositoryCarrito.GetCarritoProductoPersonalizadoAsync(carrito.IdCarrito, idProducto);
                    if (item != null)
                    {
                        item.Cantidad = nuevaCantidad;
                        await _repositoryCarrito.UpdateProductoPersonalizadoAsync(item);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error en ActualizarCantidad: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarProducto(int idUsuario, int idProducto, bool esPersonalizado = false)
        {
            try
            {
                var carrito = await _repositoryCarrito.GetByUsuarioIdAsync(idUsuario);
                if (carrito == null) return false;

                if (!esPersonalizado)
                {
                    var item = await _repositoryCarrito.GetCarritoProductoAsync(carrito.IdCarrito, idProducto);
                    if (item != null)
                    {
                        return await _repositoryCarrito.RemoveProductoAsync(item);
                    }
                }
                else
                {
                    var item = await _repositoryCarrito.GetCarritoProductoPersonalizadoAsync(carrito.IdCarrito, idProducto);
                    if (item != null)
                    {
                        return await _repositoryCarrito.RemoveProductoPersonalizadoAsync(item);
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error en EliminarProducto: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LimpiarCarrito(int idUsuario)
        {
            try
            {
                var carrito = await _repositoryCarrito.GetByUsuarioIdAsync(idUsuario);
                if (carrito == null) return false;

                return await _repositoryCarrito.LimpiarCarritoAsync(carrito.IdCarrito);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error en LimpiarCarrito: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> VerificarStock(int idUsuario)
        {
            return await Task.FromResult(true);
        }

        public async Task<int> ObtenerCantidadTotal(int idUsuario)
        {
            try
            {
                var carrito = await ObtenerCarritoUsuario(idUsuario);
                return carrito.CantidadTotal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error en ObtenerCantidadTotal: {ex.Message}");
                return 0;
            }
        }

        public async Task<decimal> ObtenerTotal(int idUsuario)
        {
            try
            {
                var carrito = await ObtenerCarritoUsuario(idUsuario);
                return carrito.TotalConImpuestos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error en ObtenerTotal: {ex.Message}");
                return 0.00m;
            }
        }
    }
}
