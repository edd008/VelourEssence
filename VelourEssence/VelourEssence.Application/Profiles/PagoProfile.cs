using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.Profiles
{
    public class PagoProfile : Profile
    {
        public PagoProfile()
        {
            CreateMap<Pago, PagoInfoDto>()
                .ForMember(dest => dest.IdPago, opt => opt.MapFrom(src => src.IdPago))
                .ForMember(dest => dest.IdPedido, opt => opt.MapFrom(src => src.IdPedido))
                .ForMember(dest => dest.TipoPago, opt => opt.MapFrom(src => src.TipoPago))
                .ForMember(dest => dest.MontoTotal, opt => opt.MapFrom(src => src.MontoTotal))
                .ForMember(dest => dest.EstadoPago, opt => opt.MapFrom(src => src.EstadoPago))
                .ForMember(dest => dest.FechaPago, opt => opt.MapFrom(src => src.FechaPago))
                .ForMember(dest => dest.NumeroTarjetaEnmascarado, opt => opt.MapFrom(src => src.NumeroTarjetaEnmascarado))
                .ForMember(dest => dest.NombreTitular, opt => opt.MapFrom(src => src.NombreTitular))
                .ForMember(dest => dest.MontoRecibido, opt => opt.MapFrom(src => src.MontoRecibido))
                .ForMember(dest => dest.Vuelto, opt => opt.MapFrom(src => src.Vuelto))
                // Mapear información del pedido
                .ForMember(dest => dest.FechaPedido, opt => opt.MapFrom(src => src.IdPedidoNavigation.FechaPedido))
                .ForMember(dest => dest.EstadoPedido, opt => opt.MapFrom(src => 
                    src.IdPedidoNavigation.IdEstadoPedido == 1 ? "Pendiente" :
                    src.IdPedidoNavigation.IdEstadoPedido == 2 ? "Pagado" :
                    src.IdPedidoNavigation.IdEstadoPedido == 3 ? "Cancelado" : "Desconocido"))
                .ForMember(dest => dest.DireccionEnvio, opt => opt.MapFrom(src => src.IdPedidoNavigation.DireccionEnvio ?? ""))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.IdPedidoNavigation.Subtotal ?? 0))
                .ForMember(dest => dest.Impuesto, opt => opt.MapFrom(src => src.IdPedidoNavigation.Impuesto ?? 0))
                .ForMember(dest => dest.CantidadProductos, opt => opt.MapFrom(src => src.IdPedidoNavigation.PedidoProducto.Count));

            CreateMap<PagoTarjetaDto, Pago>()
                .ForMember(dest => dest.TipoPago, opt => opt.MapFrom(src => src.MetodoPago.ToString()))
                .ForMember(dest => dest.MontoTotal, opt => opt.MapFrom(src => src.TotalPedido))
                .ForMember(dest => dest.NumeroTarjetaEnmascarado, opt => opt.MapFrom(src => "****" + src.NumeroTarjeta.Substring(src.NumeroTarjeta.Length - 4)))
                .ForMember(dest => dest.NombreTitular, opt => opt.MapFrom(src => src.NombreTitular))
                .ForMember(dest => dest.FechaExpiracion, opt => opt.MapFrom(src => src.FechaExpiracion))
                .ForMember(dest => dest.EstadoPago, opt => opt.MapFrom(src => "Procesando"))
                .ForMember(dest => dest.FechaPago, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<PagoEfectivoDto, Pago>()
                .ForMember(dest => dest.TipoPago, opt => opt.MapFrom(src => src.MetodoPago.ToString()))
                .ForMember(dest => dest.MontoTotal, opt => opt.MapFrom(src => src.TotalPedido))
                .ForMember(dest => dest.MontoRecibido, opt => opt.MapFrom(src => src.MontoPagado))
                .ForMember(dest => dest.Vuelto, opt => opt.MapFrom(src => src.Vuelto))
                .ForMember(dest => dest.EstadoPago, opt => opt.MapFrom(src => "Completado"))
                .ForMember(dest => dest.FechaPago, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}
