using Serilog.Events;
using Serilog;
using System.Text;
using System.Globalization;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using VelourEssence.Infraestructure.Data;
using VelourEssence.Web.Middleware;
using VelourEssence.Infraestructure.Repository.Interfaces;
using VelourEssence.Infraestructure.Repository.Implementations;
using VelourEssence.Application.Profiles;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Application.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Soporte para español (neutral) e inglés (Estados Unidos)
var supportedCultures = new[] { new CultureInfo("es"), new CultureInfo("en-US") };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("es");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // Limpiar todos los proveedores y solo usar cookie
    options.RequestCultureProviders.Clear();
    options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
});

// Configurar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "VelourEssenceAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization();

//***********************
// Configurar D.I.
//Repository

builder.Services.AddTransient<IRepositoryUsuarios, RepositoryUsuarios>();
builder.Services.AddTransient<IRepositoryEtiqueta, RepositoryEtiqueta>();
builder.Services.AddTransient<IRepositoryProductos, RepositoryProducto>();
builder.Services.AddTransient<IRepositoryPromocion, RepositoryPromocion>();
builder.Services.AddTransient<IRepositoryReseña, RepositoryReseña>();
builder.Services.AddTransient<IRepositoryPedido, RepositoryPedido>();
builder.Services.AddTransient<IRepositoryCriterioPersonalizacion, RepositoryCriterioPersonalizacion>();
builder.Services.AddTransient<IRepositoryOpcionPersonalizacion, RepositoryOpcionPersonalizacion>();
builder.Services.AddTransient<IRepositoryProductoPersonalizado, RepositoryProductoPersonalizado>();
builder.Services.AddTransient<IRepositoryCarrito, RepositoryCarrito>();

//Services

builder.Services.AddTransient<IServiceUsuario, ServiceUsuario>();
builder.Services.AddTransient<IServiceEtiqueta, ServiceEtiqueta>();
builder.Services.AddTransient<IServiceProducto, ServiceProducto>();
builder.Services.AddTransient<IServicePromocion, ServicePromocion>();
builder.Services.AddTransient<IServiceReseña, ServiceReseña>();
builder.Services.AddTransient<IPedidoService, ServicePedido>();
builder.Services.AddTransient<IServiceProductoPersonalizado, ServiceProductoPersonalizado>();
builder.Services.AddTransient<IPersonalizacionService, PersonalizacionService>();
builder.Services.AddTransient<IServiceCarrito, ServiceCarrito>();
builder.Services.AddTransient<IServicePago, ServicePago>();

//Configurar Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
/*
// ... tus perfiles de automapper
*/
// Configuar Conexión a la Base de Datos SQL
builder.Services.AddDbContext<VelourEssenceContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerDataBase"));
    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

//***********************
//Configuración Serilog
var logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(LogEventLevel.Information)
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(@"Logs\Info-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(@"Logs\Debug-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File(@"Logs\Warning-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(@"Logs\Error-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.File(@"Logs\Fatal-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .CreateLogger();

builder.Host.UseSerilog(logger);
//***************************
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseMiddleware<ErrorHandlingMiddleware>();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
