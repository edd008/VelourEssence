using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using VelourEssence.Web.Models;

namespace VelourEssence.Web.Controllers
{
    // Controlador principal de la aplicación (página de inicio, privacidad, errores)
    public class HomeController : Controller
    {
        // Logger para registrar mensajes o errores durante la ejecución
        private readonly ILogger<HomeController> _logger;

        // Constructor que recibe el logger mediante inyección de dependencias
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Acción que muestra la vista principal del sitio (página de inicio)
        public IActionResult Index()
        {
            return View();
        }

        // Acción que muestra la vista de políticas de privacidad
        public IActionResult Privacy()
        {
            return View();
        }

        // Acción para cambiar el idioma/cultura de la aplicación
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl = "/")
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }

        // Acción que muestra la vista de error cuando ocurre una excepción
        // No se almacena en caché (gracias al atributo ResponseCache)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Crea un modelo de error con el ID de la solicitud actual para mostrarlo en la vista
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
