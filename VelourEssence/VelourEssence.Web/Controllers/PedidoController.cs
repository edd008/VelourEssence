
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VelourEssence.Application.Services;
using VelourEssence.Application.DTOs;
using VelourEssence.Application.Services.Interfaces;
using System.Security.Claims;


namespace VelourEssence.Web.Controllers
{
    [Authorize] // Requiere autenticación para acceder a cualquier acción
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // GET: Pedido - Administradores ven todos los pedidos, usuarios regulares solo los suyos
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Administrador"))
            {
                // Administradores pueden ver todos los pedidos
                var todosPedidos = await _pedidoService.ObtenerTodosAsync();
                return View(todosPedidos);
            }
            else
            {
                // Usuarios regulares solo ven sus propios pedidos
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var misPedidos = await _pedidoService.ObtenerPorUsuarioAsync(userId);
                return View(misPedidos);
            }
        }


        public async Task<IActionResult> Detalle(int id)
        {
            var pedido = await _pedidoService.ObtenerPorIdAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Crear nuevo pedido
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var model = new CrearPedidoDto
            {
                Fecha = DateTime.Now,
                Estado = "Pendiente",
                ProductosDelPedido = new List<ProductoPedidoCrearDto>(),
                ProductosPersonalizadosDelPedido = new List<ProductoPersonalizadoPedidoDto>()
            };

            return View(model);
        }

        // POST: Crear nuevo pedido
        [HttpPost]
        public async Task<IActionResult> Crear(CrearPedidoDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var idPedido = await _pedidoService.CrearPedidoDirectoAsync(model, userId);
                
                TempData["SuccessMessage"] = "Pedido creado exitosamente";
                return RedirectToAction("Detalle", new { id = idPedido });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear el pedido: {ex.Message}");
                return View(model);
            }
        }

        // GET: Editar pedido existente
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var pedido = await _pedidoService.ObtenerParaEditarAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Actualizar pedido
        [HttpPost]
        public async Task<IActionResult> Editar(EditarPedidoDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var resultado = await _pedidoService.ActualizarPedidoAsync(model);
                if (resultado)
                {
                    TempData["SuccessMessage"] = "Pedido actualizado exitosamente";
                    return RedirectToAction("Detalle", new { id = model.IdPedido });
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo actualizar el pedido");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar el pedido: {ex.Message}");
                return View(model);
            }
        }

        // API: Obtener información de producto para agregar al pedido
        [HttpGet]
        public async Task<IActionResult> ObtenerProducto(int idProducto)
        {
            try
            {
                var producto = await _pedidoService.ObtenerProductoParaPedidoAsync(idProducto);
                if (producto == null)
                {
                    return Json(new { success = false, message = "Producto no encontrado" });
                }

                return Json(new { success = true, producto = producto });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Calcular totales del pedido dinámicamente
        [HttpPost]
        public async Task<IActionResult> CalcularTotales([FromBody] CalcularTotalesRequest request)
        {
            try
            {
                var totales = await _pedidoService.CalcularTotalesPedidoAsync(request);
                return Json(new { success = true, totales = totales });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Eliminar línea de detalle del pedido
        [HttpPost]
        public async Task<IActionResult> EliminarLineaDetalle(int idPedido, int tipoProducto, int idItem)
        {
            try
            {
                var resultado = await _pedidoService.EliminarLineaDetalleAsync(idPedido, tipoProducto, idItem);
                return Json(new { success = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
