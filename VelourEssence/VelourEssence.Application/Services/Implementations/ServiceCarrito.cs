using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Application.Services.Implementations
{
    public class ServiceCarrito : IServiceCarrito
    {
        private readonly IRepositoryCarrito _repositoryCarrito;
        private readonly IRepositoryProductos _repositoryProductos;
        private readonly IMapper _mapper;
        private const decimal PORCENTAJE_IMPUESTO = 0.13m;

        public ServiceCarrito(IRepositoryCarrito repositoryCarrito, IRepositoryProductos repositoryProductos, IMapper mapper)
        {
            _repositoryCarrito = repositoryCarrito;
            _repositoryProductos = repositoryProductos;
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
                    Console.WriteLine($"[DEBUG SERVICE] No existe carrito para usuario {idUsuario}, creando uno nuevo...");
                    
                    // Crear un carrito real en la base de datos en lugar de retornar uno temporal
                    carrito = new Carrito
                    {
                        IdUsuario = idUsuario,
                        FechaCreacion = DateTime.Now,
                        Activo = true
                    };
                    carrito = await _repositoryCarrito.CreateAsync(carrito);
                    
                    Console.WriteLine($"[DEBUG SERVICE] Carrito creado con ID: {carrito.IdCarrito}");
                    
                    // Retornar el carrito vacío pero con ID válido
                    return new CarritoDto
                    {
                        IdCarrito = carrito.IdCarrito,
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

                // Obtener productos normales
                var productosCarrito = await _repositoryCarrito.GetProductosCarritoAsync(carrito.IdCarrito);
                var productosDto = new List<CarritoProductoDto>();
                
                foreach (var cp in productosCarrito)
                {
                    Console.WriteLine($"[DEBUG SERVICE] ========================================");
                    Console.WriteLine($"[DEBUG SERVICE] Procesando producto ID: {cp.IdProducto}, Cantidad: {cp.Cantidad}");
                    
                    var producto = await _repositoryCarrito.GetProductoByIdAsync(cp.IdProducto);
                    if (producto != null)
                    {
                        var precioOriginal = producto.Precio ?? 100.00m;
                        var cantidad = cp.Cantidad ?? 0;
                        var precioFinal = precioOriginal;
                        var promocionTexto = "";
                        
                        Console.WriteLine($"[DEBUG SERVICE] Producto encontrado: {producto.Nombre}");
                        Console.WriteLine($"[DEBUG SERVICE] Precio original: ₡{precioOriginal:N2}");
                        
                        // Verificar promoción usando el repositorio de productos (que sabemos que funciona)
                        var promocion = await _repositoryProductos.GetPromocionActivaAsync(cp.IdProducto);
                        
                        if (promocion?.PorcentajeDescuento.HasValue == true)
                        {
                            var descuento = promocion.PorcentajeDescuento.Value;
                            precioFinal = precioOriginal * (1 - (descuento / 100));
                            promocionTexto = $" - {promocion.Nombre} (-{descuento}%)";
                            Console.WriteLine($"[DEBUG SERVICE] ✅ Promoción aplicada: {promocion.Nombre}");
                            Console.WriteLine($"[DEBUG SERVICE] ✅ Descuento: {descuento}%");
                            Console.WriteLine($"[DEBUG SERVICE] ✅ Precio con promoción: ₡{precioFinal:N2}");
                        }
                        else
                        {
                            Console.WriteLine($"[DEBUG SERVICE] ❌ Sin promoción activa para producto ID: {cp.IdProducto}");
                            Console.WriteLine($"[DEBUG SERVICE] ❌ Precio final: ₡{precioFinal:N2}");
                        }
                        
                        var productoDto = new CarritoProductoDto
                        {
                            IdProducto = cp.IdProducto,
                            NombreProducto = (producto.Nombre ?? $"Producto {cp.IdProducto}") + promocionTexto,
                            PrecioUnitario = precioFinal,
                            Cantidad = cantidad,
                            Subtotal = cantidad * precioFinal,
                            ImagenUrl = await _repositoryCarrito.GetPrimeraImagenProductoAsync(cp.IdProducto),
                            Stock = producto.Stock ?? 100
                        };
                        
                        productosDto.Add(productoDto);
                        Console.WriteLine($"[DEBUG SERVICE] ✅ Producto agregado al carrito DTO:");
                        Console.WriteLine($"[DEBUG SERVICE]    - Nombre: {productoDto.NombreProducto}");
                        Console.WriteLine($"[DEBUG SERVICE]    - Precio unitario: ₡{productoDto.PrecioUnitario:N2}");
                        Console.WriteLine($"[DEBUG SERVICE]    - Subtotal: ₡{productoDto.Subtotal:N2}");
                        Console.WriteLine($"[DEBUG SERVICE] ========================================");
                    }
                    else
                    {
                        Console.WriteLine($"[WARNING SERVICE] ❌ Producto ID {cp.IdProducto} no encontrado en base de datos");
                    }
                }

                // Obtener productos personalizados
                var productosPersonalizados = await _repositoryCarrito.GetProductosPersonalizadosCarritoAsync(carrito.IdCarrito);
                var productosPersonalizadosDto = new List<CarritoProductoPersonalizadoDto>();
                
                if (productosPersonalizados != null)
                {
                    foreach (var cpp in productosPersonalizados)
                    {
                        var productoPersonalizado = await _repositoryCarrito.GetProductoPersonalizadoByIdAsync(cpp.IdProductoPersonalizado);
                        if (productoPersonalizado != null)
                        {
                            var cantidad = cpp.Cantidad ?? 0;
                            var precio = productoPersonalizado.PrecioFinal;
                            
                            var personalizadoDto = new CarritoProductoPersonalizadoDto
                            {
                                IdProductoBase = productoPersonalizado.IdProductoBase,
                                NombreProductoBase = $"{productoPersonalizado.BaseProducto?.Nombre ?? "Producto Personalizado"} - {productoPersonalizado.Descripcion}",
                                PrecioBase = precio,
                                Cantidad = cantidad,
                                Subtotal = cantidad * precio,
                                Personalizaciones = new List<PersonalizacionSeleccionadaDto>(),
                                TotalPersonalizacion = 0.00m
                            };
                            
                            productosPersonalizadosDto.Add(personalizadoDto);
                        }
                    }
                }

                // Crear DTO del carrito
                var carritoDto = new CarritoDto
                {
                    IdCarrito = carrito.IdCarrito,
                    IdUsuario = carrito.IdUsuario ?? 0,
                    FechaCreacion = carrito.FechaCreacion ?? DateTime.Now,
                    Productos = productosDto,
                    ProductosPersonalizados = productosPersonalizadosDto
                };

                // Calcular totales
                carritoDto.Total = productosDto.Sum(p => p.Subtotal) + productosPersonalizadosDto.Sum(p => p.Subtotal);
                carritoDto.Impuestos = Math.Round(carritoDto.Total * PORCENTAJE_IMPUESTO, 2);
                carritoDto.TotalConImpuestos = carritoDto.Total + carritoDto.Impuestos;
                carritoDto.CantidadTotal = productosDto.Sum(p => p.Cantidad) + productosPersonalizadosDto.Sum(p => p.Cantidad);

                Console.WriteLine($"[DEBUG SERVICE] Carrito completo - Total: {carritoDto.Total}, Impuestos: {carritoDto.Impuestos}");
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
                
                // NUEVO: Verificar promoción antes de agregar usando el repositorio de productos
                var promocion = await _repositoryProductos.GetPromocionActivaAsync(idProducto);
                if (promocion != null)
                {
                    Console.WriteLine($"[DEBUG SERVICE] ✅ PROMOCIÓN DETECTADA al agregar producto:");
                    Console.WriteLine($"[DEBUG SERVICE]    - Promoción: {promocion.Nombre}");
                    Console.WriteLine($"[DEBUG SERVICE]    - Descuento: {promocion.PorcentajeDescuento}%");
                    Console.WriteLine($"[DEBUG SERVICE]    - Vigente desde: {promocion.FechaInicio} hasta: {promocion.FechaFin}");
                }
                else
                {
                    Console.WriteLine($"[DEBUG SERVICE] ❌ NO hay promoción activa para producto ID: {idProducto}");
                }
                
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

                var itemExistente = await _repositoryCarrito.GetCarritoProductoAsync(carrito.IdCarrito, idProducto);
                if (itemExistente != null)
                {
                    itemExistente.Cantidad = (itemExistente.Cantidad ?? 0) + cantidad;
                    await _repositoryCarrito.UpdateProductoAsync(itemExistente);
                    Console.WriteLine($"[DEBUG SERVICE] ✅ Producto existente actualizado - Nueva cantidad: {itemExistente.Cantidad}");
                }
                else
                {
                    var nuevoItem = new CarritoProducto
                    {
                        IdCarrito = carrito.IdCarrito,
                        IdProducto = idProducto,
                        Cantidad = cantidad
                    };
                    await _repositoryCarrito.AddProductoAsync(nuevoItem);
                    Console.WriteLine($"[DEBUG SERVICE] ✅ Nuevo producto agregado al carrito");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error en AgregarProducto: {ex.Message}");
                return false;
            }
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
    }
}
