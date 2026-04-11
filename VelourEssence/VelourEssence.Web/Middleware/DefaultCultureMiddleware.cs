using Microsoft.AspNetCore.Localization;

namespace VelourEssence.Web.Middleware
{
    public class DefaultCultureMiddleware
    {
        private readonly RequestDelegate _next;

        public DefaultCultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Verifica si hay una cookie de cultura establecida
            var cultureCookie = context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            
            // Si no hay cookie de cultura, establece español por defecto
            if (string.IsNullOrEmpty(cultureCookie))
            {
                context.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("es-CR")),
                    new CookieOptions 
                    { 
                        Expires = DateTimeOffset.UtcNow.AddYears(1),
                        SameSite = SameSiteMode.Lax,
                        HttpOnly = false
                    }
                );
            }

            await _next(context);
        }
    }
}
