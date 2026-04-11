using System;
using System.Collections.Generic;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.DTOs
{
    public class DashboardDto
    {
        public List<SalesByDayDto> SalesByDay { get; set; } = new();
        public List<SalesByMonthDto> SalesByMonth { get; set; } = new();
        public Dictionary<string, int> OrdersByStatus { get; set; } = new();
        public List<Producto> TopProducts { get; set; } = new();
        public List<Reseña> RecentReviews { get; set; } = new();
    }

    public class SalesByDayDto
    {
        public DateTime Fecha { get; set; }
        public int TotalVentas { get; set; }
    }

    public class SalesByMonthDto
    {
        public int Mes { get; set; }
        public int TotalVentas { get; set; }
    }
}
