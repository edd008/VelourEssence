using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelourEssence.Application.DTOs;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.Profiles
{
    public class PedidoProfile : Profile
    {
        public PedidoProfile()
        {
            CreateMap<Pedido, PedidoResumenDto>()
                .ForCtorParam("IdPedido", opt => opt.MapFrom(src => src.IdPedido))
                .ForCtorParam("NombreEstado", opt => opt.MapFrom(src => src.IdEstadoPedidoNavigation != null ? src.IdEstadoPedidoNavigation.NombreEstado : string.Empty))
                .ForCtorParam("FechaPedido", opt => opt.MapFrom(src => src.FechaPedido ?? DateTime.MinValue))
                .ForCtorParam("Total", opt => opt.MapFrom(src => CalcularTotal(src)))
                .ForCtorParam("NombreUsuario", opt => opt.MapFrom(src => src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.NombreUsuario : string.Empty));

            CreateMap<Pedido, PedidoDetalleDto>()
                .ForCtorParam("IdPedido", opt => opt.MapFrom(src => src.IdPedido))
                .ForCtorParam("Estado", opt => opt.MapFrom(src => src.IdEstadoPedidoNavigation != null ? src.IdEstadoPedidoNavigation.NombreEstado : null))
                .ForCtorParam("Fecha", opt => opt.MapFrom(src => src.FechaPedido))
                .ForCtorParam("DireccionEnvio", opt => opt.MapFrom(src => src.DireccionEnvio))
                .ForCtorParam("MetodoPago", opt => opt.MapFrom(src => src.MetodoPago))
                .ForCtorParam("Productos", opt => opt.MapFrom(src => src.PedidoProducto))
                .ForCtorParam("Subtotal", opt => opt.MapFrom(src => CalcularSubtotal(src)))
                .ForCtorParam("Impuesto", opt => opt.MapFrom(src => CalcularImpuesto(src)))
                .ForCtorParam("Total", opt => opt.MapFrom(src => CalcularTotal(src)))
                .ForCtorParam("NombreCliente", opt => opt.MapFrom(src => src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.NombreUsuario : null));

            CreateMap<PedidoProducto, ProductoPedidoDto>()
                .ForCtorParam("Nombre", opt => opt.MapFrom(src => src.IdProductoNavigation != null ? src.IdProductoNavigation.Nombre : null))
                .ForCtorParam("Cantidad", opt => opt.MapFrom(src => src.Cantidad))
                .ForCtorParam("PrecioUnitario", opt => opt.MapFrom(src => src.PrecioUnitario));
        }

        /// <summary>
        /// Calcula el subtotal basado en los productos del pedido
        /// </summary>
        private static decimal CalcularSubtotal(Pedido pedido)
        {
            if (pedido?.PedidoProducto == null || !pedido.PedidoProducto.Any())
                return 0;

            return pedido.PedidoProducto.Sum(pp => (pp.Cantidad ?? 0) * (pp.PrecioUnitario ?? 0));
        }

        /// <summary>
        /// Calcula el impuesto (13%) basado en el subtotal
        /// </summary>
        private static decimal CalcularImpuesto(Pedido pedido)
        {
            var subtotal = CalcularSubtotal(pedido);
            return Math.Round(subtotal * 0.13m, 2);
        }

        /// <summary>
        /// Calcula el total (subtotal + impuesto)
        /// </summary>
        private static decimal CalcularTotal(Pedido pedido)
        {
            var subtotal = CalcularSubtotal(pedido);
            var impuesto = CalcularImpuesto(pedido);
            return subtotal + impuesto;
        }
    }
}