using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VelourEssence.Web.Controllers
{
    [Authorize]
    public class TestSimpleController : Controller
    {
        // Método de prueba muy básico
        [HttpGet]
        [HttpPost]
        public IActionResult TestAgregar(int idProducto = 1, int cantidad = 1)
        {
            try
            {
                Console.WriteLine($"[TEST] Producto: {idProducto}, Cantidad: {cantidad}");
                
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"[TEST] Usuario: {userId}");
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Usuario no autenticado" });
                }

                // Simulación exitosa
                return Json(new { 
                    success = true, 
                    message = "Producto agregado exitosamente (PRUEBA)",
                    cantidadTotal = 1,
                    idProducto = idProducto,
                    cantidad = cantidad,
                    usuario = userId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TEST ERROR] {ex.Message}");
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}
