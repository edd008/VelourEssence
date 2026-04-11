using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelourEssence.Application.DTOs;
using VelourEssence.Infraestructure.Data;
using VelourEssence.Infraestructure.Models;
using System.Linq;

namespace VelourEssence.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly VelourEssenceContext _context;

        public DashboardController(VelourEssenceContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new DashboardDto();

            // Ventas por día (últimos 30 días)
            model.SalesByDay = _context.Pedido
                .Where(p => p.FechaPedido.HasValue && p.FechaPedido.Value >= DateTime.Today.AddDays(-30))
                .GroupBy(p => p.FechaPedido.Value.Date)
                .Select(g => new SalesByDayDto
                {
                    Fecha = g.Key,
                    TotalVentas = g.Count()
                })
                .OrderBy(s => s.Fecha)
                .ToList();

            // Ventas por mes (últimos 12 meses)
            model.SalesByMonth = _context.Pedido
                .Where(p => p.FechaPedido.HasValue && p.FechaPedido.Value >= DateTime.Today.AddMonths(-12))
                .GroupBy(p => p.FechaPedido.Value.Month)
                .Select(g => new SalesByMonthDto
                {
                    Mes = g.Key,
                    TotalVentas = g.Count()
                })
                .OrderBy(s => s.Mes)
                .ToList();

            // Pedidos por estado
            model.OrdersByStatus = _context.Pedido
                .Include(p => p.IdEstadoPedidoNavigation)
                .GroupBy(p => p.IdEstadoPedidoNavigation!.NombreEstado)
                .ToDictionary(g => g.Key, g => g.Count());

            // 3 productos más vendidos
            model.TopProducts = _context.PedidoProducto
                .Include(pp => pp.IdProductoNavigation)
                .GroupBy(pp => pp.IdProductoNavigation!)
                .OrderByDescending(g => g.Sum(x => x.Cantidad))
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            // 3 reseñas más recientes
            model.RecentReviews = _context.Reseña
                .Include(r => r.IdUsuarioNavigation)
                .OrderByDescending(r => r.Fecha)
                .Take(3)
                .ToList();

            return View(model);
        }
    }
}
