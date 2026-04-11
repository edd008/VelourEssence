using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VelourEssence.Application.DTOs;
using VelourEssence.Application.Services.Interfaces;

namespace VelourEssence.Web.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IServicePago _servicePago;
        private readonly IServiceCarrito _carritoService;
        private readonly IPedidoService _pedidoService;

        public PagoController(IServicePago servicePago, IServiceCarrito carritoService, IPedidoService pedidoService)
        {
            _servicePago = servicePago;
            _carritoService = carritoService;
            _pedidoService = pedidoService;
        }

        // GET: /Pago/SeleccionarMetodo/{idCarrito}
        public async Task<IActionResult> SeleccionarMetodo(int idCarrito)
        {
            try
            {
                // Verificar que el carrito existe y tiene productos
                // Por ahora simulamos que idCarrito = idUsuario
                var carrito = await _carritoService.ObtenerCarritoUsuario(idCarrito);
                if (carrito == null || (!carrito.Productos.Any() && !carrito.ProductosPersonalizados.Any()))
                {
                    TempData["Error"] = "El carrito está vacío o no existe";
                    return RedirectToAction("Index", "Carrito");
                }

                var totalProductos = carrito.Productos.Sum(cp => cp.Subtotal);
                var totalPersonalizados = carrito.ProductosPersonalizados.Sum(cp => cp.Subtotal);

                var model = new SeleccionMetodoPagoDto
                {
                    IdCarrito = idCarrito,
                    TotalPedido = carrito.TotalConImpuestos, // Usar el total con impuestos del carrito
                    MetodosPagoDisponibles = new List<string> { "TarjetaCredito", "TarjetaDebito", "Efectivo" }
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar los métodos de pago: {ex.Message}";
                return RedirectToAction("Index", "Carrito");
            }
        }

        // GET: /Pago/PagarConTarjeta/{idCarrito}
        public async Task<IActionResult> PagarConTarjeta(int idCarrito, string metodo = "TarjetaCredito")
        {
            try
            {
                // Obtener el carrito completo para acceder al total con impuestos
                var carrito = await _carritoService.ObtenerCarritoUsuario(idCarrito);
                if (carrito == null || carrito.TotalConImpuestos <= 0)
                {
                    TempData["Error"] = "El carrito está vacío o no existe";
                    return RedirectToAction("Index", "Carrito");
                }

                var model = new PagoTarjetaDto
                {
                    IdCarrito = idCarrito,
                    TotalPedido = carrito.TotalConImpuestos, // Usar total con impuestos
                    MetodoPago = metodo == "TarjetaDebito" ? MetodoPago.TarjetaDebito : MetodoPago.TarjetaCredito
                };

                ViewBag.MetodoPago = metodo;
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el formulario de pago: {ex.Message}";
                return RedirectToAction("SeleccionarMetodo", new { idCarrito });
            }
        }

        // POST: /Pago/ProcesarPagoTarjeta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcesarPagoTarjeta(PagoTarjetaDto model)
        {
            Console.WriteLine("🚀 [CONTROLLER] ¡ProcesarPagoTarjeta LLAMADO!");
            Console.WriteLine($"📋 [CONTROLLER] Model recibido: IdCarrito={model?.IdCarrito}, TotalPedido={model?.TotalPedido}, MetodoPago={model?.MetodoPago}");
            Console.WriteLine($"🔍 [CONTROLLER] Datos tarjeta: Numero={model?.NumeroTarjeta?.Length} chars, Nombre={model?.NombreTitular?.Length} chars");
            
            try
            {
                if (model == null)
                {
                    Console.WriteLine("❌ [CONTROLLER] Model es NULL!");
                    ModelState.AddModelError("", "Datos del formulario inválidos");
                    return View("PagarConTarjeta");
                }
                
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("❌ [CONTROLLER] ModelState no válido");
                    foreach (var error in ModelState)
                    {
                        Console.WriteLine($"[DEBUG CONTROLLER] Error en {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                    return View("PagarConTarjeta", model);
                }

                Console.WriteLine("[DEBUG CONTROLLER] ModelState válido, procesando pago...");
                
                // USAR EXACTAMENTE LA MISMA LÓGICA QUE EFECTIVO
                var resultado = await _servicePago.ProcesarPagoTarjetaAsync(model);
                
                Console.WriteLine($"[DEBUG CONTROLLER] Resultado: Exitoso={resultado.Exitoso}, Mensaje={resultado.Mensaje}");
                Console.WriteLine($"[DEBUG CONTROLLER] IdPago={resultado.IdPago}, IdPedido={resultado.IdPedido}");
                
                if (resultado.Exitoso)
                {
                    Console.WriteLine("[DEBUG CONTROLLER] Pago exitoso, configurando TempData y redirigiendo...");
                    TempData["Success"] = resultado.Mensaje;
                    TempData["NumeroTransaccion"] = resultado.NumeroTransaccion;
                    // No hay vuelto en tarjetas
                    
                    Console.WriteLine($"[DEBUG CONTROLLER] Redirigiendo a Confirmacion con IdPago={resultado.IdPago}, NumeroTransaccion={resultado.NumeroTransaccion}");
                    return RedirectToAction("Confirmacion", new { 
                        idPago = resultado.IdPago, 
                        numeroTransaccion = resultado.NumeroTransaccion 
                    });
                }
                else
                {
                    Console.WriteLine($"[DEBUG CONTROLLER] Pago fallido: {resultado.Mensaje}");
                    ModelState.AddModelError("", resultado.Mensaje);
                    return View("PagarConTarjeta", model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR CONTROLLER] Error al procesar el pago: {ex.Message}");
                Console.WriteLine($"[ERROR CONTROLLER] StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Error al procesar el pago: {ex.Message}");
                return View("PagarConTarjeta", model);
            }
        }

        // GET: /Pago/PagarEnEfectivo/{idCarrito}
        public async Task<IActionResult> PagarEnEfectivo(int idCarrito)
        {
            try
            {
                // Obtener el carrito completo para acceder al total con impuestos
                var carrito = await _carritoService.ObtenerCarritoUsuario(idCarrito);
                if (carrito == null || carrito.TotalConImpuestos <= 0)
                {
                    TempData["Error"] = "El carrito está vacío o no existe";
                    return RedirectToAction("Index", "Carrito");
                }

                var model = new PagoEfectivoDto
                {
                    IdCarrito = idCarrito,
                    TotalPedido = carrito.TotalConImpuestos, // Usar total con impuestos
                    MetodoPago = MetodoPago.Efectivo
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el formulario de pago: {ex.Message}";
                return RedirectToAction("SeleccionarMetodo", new { idCarrito });
            }
        }

        // POST: /Pago/ProcesarPagoEfectivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcesarPagoEfectivo(PagoEfectivoDto model)
        {
            Console.WriteLine($"[DEBUG CONTROLLER] ProcesarPagoEfectivo llamado");
            Console.WriteLine($"[DEBUG CONTROLLER] Model: IdCarrito={model.IdCarrito}, TotalPedido={model.TotalPedido}, MontoPagado={model.MontoPagado}");
            
            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("[DEBUG CONTROLLER] ModelState no válido");
                    foreach (var error in ModelState)
                    {
                        Console.WriteLine($"[DEBUG CONTROLLER] Error en {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                    return View("PagarEnEfectivo", model);
                }

                Console.WriteLine("[DEBUG CONTROLLER] ModelState válido, procesando pago...");
                
                // Procesar el pago (el vuelto se calcula automáticamente en el DTO)
                var resultado = await _servicePago.ProcesarPagoEfectivoAsync(model);
                
                Console.WriteLine($"[DEBUG CONTROLLER] Resultado: Exitoso={resultado.Exitoso}, Mensaje={resultado.Mensaje}");
                Console.WriteLine($"[DEBUG CONTROLLER] IdPago={resultado.IdPago}, IdPedido={resultado.IdPedido}");
                
                if (resultado.Exitoso)
                {
                    Console.WriteLine("[DEBUG CONTROLLER] Pago exitoso, configurando TempData y redirigiendo...");
                    TempData["Success"] = resultado.Mensaje;
                    TempData["NumeroTransaccion"] = resultado.NumeroTransaccion;
                    TempData["Vuelto"] = resultado.Vuelto?.ToString("F2") ?? "0.00"; // Convertir a string para TempData
                    
                    Console.WriteLine($"[DEBUG CONTROLLER] Redirigiendo a Confirmacion con IdPago={resultado.IdPago}, NumeroTransaccion={resultado.NumeroTransaccion}");
                    return RedirectToAction("Confirmacion", new { 
                        idPago = resultado.IdPago, 
                        numeroTransaccion = resultado.NumeroTransaccion 
                    });
                }
                else
                {
                    Console.WriteLine($"[DEBUG CONTROLLER] Pago fallido: {resultado.Mensaje}");
                    ModelState.AddModelError("", resultado.Mensaje);
                    return View("PagarEnEfectivo", model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR CONTROLLER] Error al procesar el pago: {ex.Message}");
                Console.WriteLine($"[ERROR CONTROLLER] StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Error al procesar el pago: {ex.Message}");
                return View("PagarEnEfectivo", model);
            }
        }

        // GET: /Pago/Confirmacion/{idPago}/{numeroTransaccion}
        public async Task<IActionResult> Confirmacion(int idPago, string numeroTransaccion)
        {
            Console.WriteLine($"[DEBUG CONTROLLER] Confirmacion llamada con IdPago={idPago}, NumeroTransaccion={numeroTransaccion}");
            
            try
            {
                var pago = await _servicePago.ObtenerPagoPorIdAsync(idPago);
                Console.WriteLine($"[DEBUG CONTROLLER] Pago obtenido: {(pago != null ? "Encontrado" : "No encontrado")}");
                
                if (pago == null)
                {
                    Console.WriteLine("[DEBUG CONTROLLER] Pago no encontrado, redirigiendo a Home");
                    TempData["Error"] = "No se encontró la información del pago";
                    return RedirectToAction("Index", "Home");
                }

                Console.WriteLine($"[DEBUG CONTROLLER] Pago encontrado - IdPedido={pago.IdPedido}, Monto={pago.MontoTotal}");
                
                var model = new ConfirmacionPagoDto
                {
                    Pago = pago,
                    NumeroTransaccion = numeroTransaccion,
                    MensajeExito = TempData["Success"]?.ToString(),
                    Vuelto = decimal.TryParse(TempData["Vuelto"]?.ToString(), out var vuelto) ? vuelto : (decimal?)null
                };

                Console.WriteLine($"[DEBUG CONTROLLER] Modelo de confirmación creado exitosamente");
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR CONTROLLER] Error en Confirmacion: {ex.Message}");
                TempData["Error"] = $"Error al cargar la confirmación: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /Pago/HistorialPagos
        public async Task<IActionResult> HistorialPagos()
        {
            try
            {
                // Obtener el ID del usuario autenticado
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"[DEBUG] User ID Claim: {userIdClaim}");
                
                if (!int.TryParse(userIdClaim, out int idUsuario))
                {
                    Console.WriteLine("[DEBUG] No se pudo parsear el ID del usuario, redirigiendo a login");
                    return RedirectToAction("Login", "Auth");
                }

                Console.WriteLine($"[DEBUG] Obteniendo pagos para usuario ID: {idUsuario}");
                
                // Obtener los pagos reales del usuario
                var pagos = await _servicePago.ObtenerPagosPorUsuarioAsync(idUsuario);
                
                Console.WriteLine($"[DEBUG] Pagos encontrados: {pagos.Count}");
                foreach (var pago in pagos)
                {
                    Console.WriteLine($"[DEBUG] Pago ID: {pago.IdPago}, Pedido ID: {pago.IdPedido}, Monto: {pago.MontoTotal}, Fecha: {pago.FechaPago}");
                }
                
                return View(pagos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error al cargar el historial: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
                TempData["Error"] = $"Error al cargar el historial: {ex.Message}";
                return View(new List<PagoInfoDto>());
            }
        }

        // GET: /Pago/CrearDatosPrueba - Método temporal para crear datos de prueba
        public async Task<IActionResult> CrearDatosPrueba()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int idUsuario))
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Crear datos de prueba si no existen
                Console.WriteLine($"[DEBUG] Creando datos de prueba para usuario {idUsuario}");
                
                TempData["Success"] = "Método de datos de prueba ejecutado. Revisa la consola y luego ve al historial de pagos.";
                return RedirectToAction("HistorialPagos");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("HistorialPagos");
            }
        }

        // AJAX: /Pago/ValidarTarjeta
        [HttpPost]
        public async Task<JsonResult> ValidarTarjeta([FromBody] PagoTarjetaDto model)
        {
            try
            {
                var validacion = await _servicePago.ValidarTarjetaAsync(model);
                return Json(new { 
                    esValida = validacion.EsValida, 
                    errores = validacion.Errores,
                    tipoTarjeta = validacion.TipoTarjeta
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    esValida = false, 
                    errores = new List<string> { $"Error al validar: {ex.Message}" }
                });
            }
        }

        // AJAX: /Pago/CalcularVuelto
        [HttpPost]
        public JsonResult CalcularVuelto(decimal totalPedido, decimal montoPagado)
        {
            try
            {
                var vuelto = montoPagado - totalPedido;
                var esSuficiente = montoPagado >= totalPedido;
                
                return Json(new { 
                    vuelto = vuelto >= 0 ? vuelto : 0, 
                    esSuficiente = esSuficiente,
                    mensaje = esSuficiente ? "Monto suficiente" : "Monto insuficiente"
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    vuelto = 0, 
                    esSuficiente = false,
                    mensaje = $"Error: {ex.Message}"
                });
            }
        }
    }
}
