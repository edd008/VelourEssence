using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Application.DTOs;
using System.Text.Json;
using System.Security.Claims;

namespace VelourEssence.Web.Controllers

{
    // Controlador que maneja las vistas relacionadas con reseñas
    public class ReseñaController : Controller
    {
        // Servicio para acceder a los datos de reseñas
        private readonly IServiceReseña _serviceReseña;

        // Constructor que recibe el servicio de reseñas por inyección de dependencias
        public ReseñaController(IServiceReseña serviceReseña)
        {
            _serviceReseña = serviceReseña;
        }

        // Acción que muestra la lista de todas las reseñas
        public async Task<IActionResult> Index()
        {
            var reseñas = await _serviceReseña.ListAsync();
            return View(reseñas); 
        }

        // Acción que muestra los detalles de una reseña específica por su ID
        public async Task<IActionResult> Details(int id)
        {
            var reseña = await _serviceReseña.GetByIdAsync(id);

            // Si no se encuentra la reseña, devuelve un error 404
            if (reseña == null)
                return NotFound();

            // Si existe, envía la reseña a la vista para mostrar detalles
            return View(reseña);
        }

        // Acción POST para crear una nueva reseña via AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Solo usuarios autenticados pueden crear reseñas
        public async Task<IActionResult> Create(CrearReseñaDto crearReseñaDto)
        {
            try
            {
                // Obtener el ID del usuario autenticado
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Json(new { 
                        success = false, 
                        message = "Debes estar logueado para crear una reseña"
                    });
                }

                // Asignar el ID del usuario autenticado
                crearReseñaDto.IdUsuario = userId;

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => new { 
                            Field = x.Key, 
                            Messages = x.Value?.Errors.Select(e => e.ErrorMessage) ?? new List<string>()
                        });

                    return Json(new { 
                        success = false, 
                        message = "Por favor, corrige los errores en el formulario",
                        errors = errors
                    });
                }

                var nuevaReseña = await _serviceReseña.CreateAsync(crearReseñaDto);

                // Calcular nuevo promedio después de crear la reseña
                var nuevoPromedio = await _serviceReseña.GetAverageRatingByProductIdAsync(crearReseñaDto.IdProducto);

                // Obtener el nombre del usuario autenticado
                var nombreUsuario = User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuario";

                

                return Json(new
                {
                    success = true,
                    message = "¡Reseña creada exitosamente!",
                    data = new
                    {
                        reseña = new
                        {
                            usuario = nombreUsuario,
                            comentario = nuevaReseña.Comentario,
                            valoracion = nuevaReseña.Valoracion,
                            fecha = nuevaReseña.Fecha?.ToString("dd/MM/yyyy") ?? ""
                        },
                        nuevoPromedio = Math.Round(nuevoPromedio, 1)
                    }
                });


            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Ocurrió un error al crear la reseña. Por favor, inténtalo de nuevo.",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }



        }

        // Acción para obtener reseñas de un producto específico via AJAX
        [HttpGet]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            try
            {
                var reseñas = await _serviceReseña.GetByProductIdAsync(productId);
                var promedio = await _serviceReseña.GetAverageRatingByProductIdAsync(productId);

                return Json(new { 
                    success = true, 
                    data = new {
                        reseñas = reseñas,
                        promedio = Math.Round(promedio, 1)
                    }
                });
            }
            catch (Exception)
            {
                return Json(new { 
                    success = false, 
                    message = "Error al cargar las reseñas"
                });
            }
        }
    }
}
