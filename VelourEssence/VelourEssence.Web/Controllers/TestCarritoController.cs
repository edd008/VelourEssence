using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VelourEssence.Application.Services.Interfaces;

namespace VelourEssence.Web.Controllers
{
    [Authorize] // Requiere autenticación
    public class TestCarritoController : Controller
    {
        private readonly IServiceCarrito _serviceCarrito;

        public TestCarritoController(IServiceCarrito serviceCarrito)
        {
            _serviceCarrito = serviceCarrito;
        }

        // Página de prueba para agregar productos al carrito
        public IActionResult Index()
        {
            return View();
        }

        // Método de prueba para agregar producto
        [HttpPost]
        public async Task<IActionResult> AgregarProductoPrueba(int idProducto = 1, int cantidad = 1)
        {
            var idUsuario = ObtenerIdUsuario();
            if (idUsuario == 0)
                return Json(new { success = false, message = "Debe iniciar sesión para agregar productos al carrito." });

            var resultado = await _serviceCarrito.AgregarProducto(idUsuario, idProducto, cantidad);
            
            if (resultado)
            {
                var cantidadTotal = await _serviceCarrito.ObtenerCantidadTotal(idUsuario);
                return Json(new { 
                    success = true, 
                    message = "Producto agregado al carrito exitosamente.",
                    cantidadTotal = cantidadTotal
                });
            }
            
            return Json(new { success = false, message = "Error al agregar el producto al carrito." });
        }

        // Método para verificar el carrito
        [HttpGet]
        public async Task<IActionResult> VerificarCarrito()
        {
            var idUsuario = ObtenerIdUsuario();
            if (idUsuario == 0)
                return Json(new { error = "Usuario no autenticado" });

            var carrito = await _serviceCarrito.ObtenerCarritoUsuario(idUsuario);
            return Json(new { 
                idCarrito = carrito.IdCarrito,
                cantidadTotal = carrito.CantidadTotal,
                productos = carrito.Productos.Count,
                productosPersonalizados = carrito.ProductosPersonalizados.Count
            });
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
