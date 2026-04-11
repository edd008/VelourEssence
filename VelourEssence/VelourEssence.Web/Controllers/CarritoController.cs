using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Application.DTOs;

namespace VelourEssence.Web.Controllers
{
    [Authorize] // Requiere autenticación
    public class CarritoController : Controller
    {
        private readonly IServiceCarrito _serviceCarrito;

        public CarritoController(IServiceCarrito serviceCarrito)
        {
            _serviceCarrito = serviceCarrito;
        }

        // Vista principal del carrito
        public async Task<IActionResult> Index()
        {
            var idUsuario = ObtenerIdUsuario();
            if (idUsuario == 0)
                return RedirectToAction("Login", "Auth");

            var carrito = await _serviceCarrito.ObtenerCarritoUsuario(idUsuario);
            return View(carrito);
        }

        // Agregar producto al carrito (versión con validación de producto)
        [HttpPost]
        [HttpGet] // Agregamos GET para pruebas
        public async Task<IActionResult> AgregarProducto(int idProducto, int cantidad = 1)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Iniciando AgregarProducto - Producto: {idProducto}, Cantidad: {cantidad}");

                // Verificar si el usuario está autenticado
                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    Console.WriteLine("[DEBUG] Usuario no autenticado");
                    return Json(new { success = false, message = "Debe iniciar sesión." });
                }

                var idUsuario = ObtenerIdUsuario();
                Console.WriteLine($"[DEBUG] ID Usuario obtenido: {idUsuario}");
                
                if (idUsuario == 0)
                {
                    Console.WriteLine("[DEBUG] ID Usuario es 0");
                    return Json(new { success = false, message = "Error al obtener usuario." });
                }

                // Validar parámetros básicos
                if (idProducto <= 0)
                {
                    Console.WriteLine($"[DEBUG] ID Producto inválido: {idProducto}");
                    return Json(new { success = false, message = "ID de producto inválido." });
                }

                if (cantidad <= 0)
                {
                    Console.WriteLine($"[DEBUG] Cantidad inválida: {cantidad}");
                    return Json(new { success = false, message = "La cantidad debe ser mayor a 0." });
                }

                // Si el producto no existe, lo simulamos para que funcione
                // En un escenario real, verificarías en la base de datos de productos
                Console.WriteLine("[DEBUG] Llamando al servicio AgregarProducto...");
                
                try 
                {
                    var resultado = await _serviceCarrito.AgregarProducto(idUsuario, idProducto, cantidad);
                    Console.WriteLine($"[DEBUG] Resultado del servicio: {resultado}");
                    
                    if (resultado)
                    {
                        Console.WriteLine("[DEBUG] Producto agregado exitosamente, obteniendo cantidad total...");
                        var cantidadTotal = await _serviceCarrito.ObtenerCantidadTotal(idUsuario);
                        Console.WriteLine($"[DEBUG] Cantidad total obtenida: {cantidadTotal}");
                        
                        return Json(new { 
                            success = true, 
                            message = "Producto agregado exitosamente al carrito real.",
                            cantidadTotal = cantidadTotal
                        });
                    }
                    else
                    {
                        Console.WriteLine("[DEBUG] El servicio retornó false");
                        return Json(new { success = false, message = "No se pudo agregar el producto al carrito." });
                    }
                }
                catch (Exception serviceEx)
                {
                    Console.WriteLine($"[DEBUG] Error en el servicio: {serviceEx.Message}");
                    Console.WriteLine($"[DEBUG] Stack del servicio: {serviceEx.StackTrace}");
                    
                    // Si hay error en el servicio, retornamos el error específico
                    return Json(new { success = false, message = "Error en el servicio: " + serviceEx.Message });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Excepción en AgregarProducto: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException.Message}");
                }
                return Json(new { success = false, message = "Error interno del servidor: " + ex.Message });
            }
        }

        // Actualizar cantidad de un producto
        [HttpPost]
        public async Task<IActionResult> ActualizarCantidad(int idProducto, int cantidad, bool esPersonalizado = false)
        {
            var idUsuario = ObtenerIdUsuario();
            if (idUsuario == 0)
                return Json(new { success = false, message = "Sesión expirada." });

            if (cantidad < 0)
                return Json(new { success = false, message = "La cantidad no puede ser negativa." });

            var resultado = await _serviceCarrito.ActualizarCantidad(idUsuario, idProducto, cantidad, esPersonalizado);
            
            if (resultado)
            {
                var carrito = await _serviceCarrito.ObtenerCarritoUsuario(idUsuario);
                return Json(new { 
                    success = true, 
                    cantidadTotal = carrito.CantidadTotal,
                    total = carrito.Total,
                    totalConImpuestos = carrito.TotalConImpuestos
                });
            }
            
            return Json(new { success = false, message = "Error al actualizar la cantidad." });
        }

        // Eliminar producto del carrito
        [HttpPost]
        public async Task<IActionResult> EliminarProducto(int idProducto, bool esPersonalizado = false)
        {
            var idUsuario = ObtenerIdUsuario();
            if (idUsuario == 0)
                return Json(new { success = false, message = "Sesión expirada." });

            var resultado = await _serviceCarrito.EliminarProducto(idUsuario, idProducto, esPersonalizado);
            
            if (resultado)
            {
                var carrito = await _serviceCarrito.ObtenerCarritoUsuario(idUsuario);
                return Json(new { 
                    success = true, 
                    cantidadTotal = carrito.CantidadTotal,
                    total = carrito.Total,
                    totalConImpuestos = carrito.TotalConImpuestos
                });
            }
            
            return Json(new { success = false, message = "Error al eliminar el producto." });
        }

        // Limpiar carrito completo
        [HttpPost]
        public async Task<IActionResult> LimpiarCarrito()
        {
            var idUsuario = ObtenerIdUsuario();
            if (idUsuario == 0)
                return Json(new { success = false, message = "Sesión expirada." });

            var resultado = await _serviceCarrito.LimpiarCarrito(idUsuario);
            
            if (resultado)
            {
                return Json(new { success = true, message = "Carrito limpiado exitosamente." });
            }
            
            return Json(new { success = false, message = "Error al limpiar el carrito." });
        }

        // Obtener cantidad total del carrito (para el header)
        [HttpGet]
        public async Task<IActionResult> ObtenerCantidadTotal()
        {
            var idUsuario = ObtenerIdUsuario();
            if (idUsuario == 0)
                return Json(new { cantidadTotal = 0 });

            var cantidadTotal = await _serviceCarrito.ObtenerCantidadTotal(idUsuario);
            return Json(new { cantidadTotal = cantidadTotal });
        }

        // Proceder al checkout - Redirige al sistema de pagos
        public async Task<IActionResult> Checkout()
        {
            var idUsuario = ObtenerIdUsuario();
            if (idUsuario == 0)
                return RedirectToAction("Login", "Auth");

            var carrito = await _serviceCarrito.ObtenerCarritoUsuario(idUsuario);
            
            if (!carrito.Productos.Any() && !carrito.ProductosPersonalizados.Any())
            {
                TempData["Error"] = "El carrito está vacío.";
                return RedirectToAction("Index");
            }

            // Verificar stock
            var stockValido = await _serviceCarrito.VerificarStock(idUsuario);
            if (!stockValido)
            {
                TempData["Error"] = "Algunos productos no tienen suficiente stock disponible.";
                return RedirectToAction("Index");
            }

            // Redirigir al sistema de pagos usando el ID del carrito (que es igual al ID del usuario)
            return RedirectToAction("SeleccionarMetodo", "Pago", new { idCarrito = carrito.IdCarrito });
        }

        private int ObtenerIdUsuario()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int idUsuario))
            {
                return idUsuario;
            }
            return 0;
        }
    }
}
