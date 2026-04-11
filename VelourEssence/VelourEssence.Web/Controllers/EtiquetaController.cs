using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VelourEssence.Application.Services.Interfaces;

namespace VelourEssence.Web.Controllers
{
    // Controlador encargado de manejar las vistas relacionadas con las etiquetas - Solo administradores
    [Authorize(Roles = "Administrador")]
    public class EtiquetaController : Controller
    {
        // Servicio que permite acceder a los datos de etiquetas
        private readonly IServiceEtiqueta _serviceEtiqueta;

        // Constructor que recibe el servicio de etiquetas por inyección de dependencias
        public EtiquetaController(IServiceEtiqueta serviceEtiqueta)
        {
            _serviceEtiqueta = serviceEtiqueta;
        }

        // Acción que muestra la lista de todas las etiquetas
        // Llama al servicio para obtener los datos y los pasa a la vista
        public async Task<IActionResult> Index()
        {
            var etiquetas = await _serviceEtiqueta.ListAsync();
            return View(etiquetas); 
        }

        // Acción que muestra los detalles de una etiqueta específica por su ID
        public async Task<IActionResult> Details(int id)
        {
            var etiqueta = await _serviceEtiqueta.GetByIdAsync(id);
            if (etiqueta == null)
                return NotFound();
            return View(etiqueta);
        }

        // GET: Etiqueta/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Etiqueta/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VelourEssence.Application.DTOs.EtiquetaDto dto)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (ModelState.IsValid)
                {
                    await _serviceEtiqueta.CreateAsync(dto);
                    return Json(new { success = true, message = "Etiqueta creada exitosamente." });
                }
                var errorMsg = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, error = errorMsg });
            }
            // Flujo tradicional
            if (ModelState.IsValid)
            {
                await _serviceEtiqueta.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: Etiqueta/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var etiqueta = await _serviceEtiqueta.GetByIdAsync(id);
            if (etiqueta == null)
                return NotFound();
            return View(etiqueta);
        }

        // POST: Etiqueta/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VelourEssence.Application.DTOs.EtiquetaDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceEtiqueta.UpdateAsync(dto);
                if (result)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "No se pudo actualizar la etiqueta.");
            }
            return View(dto);
        }
            [HttpPost]
            public async Task<IActionResult> Delete(int id)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var result = await _serviceEtiqueta.DeleteAsync(id);
                    if (result)
                        return Json(new { success = true, message = "Etiqueta eliminada exitosamente." });
                    return Json(new { success = false, error = "No se pudo eliminar la etiqueta." });
                }
                // Flujo tradicional
                var deleteResult = await _serviceEtiqueta.DeleteAsync(id);
                if (deleteResult)
                    return RedirectToAction(nameof(Index));
                return BadRequest();
            }
    }
}
