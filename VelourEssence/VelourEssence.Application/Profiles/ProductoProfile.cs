using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Application.Profiles
{
    // Profile de AutoMapper para mapear entidad Producto y relacionadas a DTOs
    public class ProductoProfile : Profile
    {
        public ProductoProfile()
        {
            // Mapeo de Producto a ProductoDetalleDto (información completa)
            CreateMap<Producto, ProductoDetalleDto>()
                // Mapea nombre de categoría desde navegación
                .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src =>
                    src.IdCategoriaNavigation != null ? src.IdCategoriaNavigation.Nombre : string.Empty))
                // Mapea lista de nombres de etiquetas
                .ForMember(dest => dest.Etiquetas, opt => opt.MapFrom(src =>
                    src.IdEtiqueta.Select(e => e.Nombre).ToList()))
                // Mapea imágenes del producto
                .ForMember(dest => dest.Imagenes, opt => opt.MapFrom(src => src.ImagenProducto))
                // Calcula promedio de valoraciones
                .ForMember(dest => dest.PromedioValoracion, opt => opt.MapFrom(src =>
                    src.Reseña != null && src.Reseña.Any()
                        ? src.Reseña.Average(r => r.Valoracion ?? 0)
                        : 0))
                // Mapea reseñas del producto
                .ForMember(dest => dest.Resenas, opt => opt.MapFrom(src => src.Reseña))
                // Verifica si tiene promoción activa
                .ForMember(dest => dest.TienePromocion, opt => opt.MapFrom(src => TienePromocionActiva(src)))
                // Calcula precio con descuento aplicado
                .ForMember(dest => dest.PrecioConDescuento, opt => opt.MapFrom(src => CalcularPrecioConDescuento(src)));

            // Mapeo de Producto a ProductoDto (vista resumida para listados)
            CreateMap<Producto, ProductoDto>()
                .ForMember(dest => dest.Imagenes, opt => opt.MapFrom(src => src.ImagenProducto))
                .ForMember(dest => dest.TienePromocion, opt => opt.MapFrom(src => TienePromocionActiva(src)))
                .ForMember(dest => dest.PrecioConDescuento, opt => opt.MapFrom(src => CalcularPrecioConDescuento(src)));

            // Mapeo de ImagenProducto a ImagenProductoDto
            CreateMap<ImagenProducto, ImagenProductoDto>()
                .ForMember(dest => dest.IdImagenProducto, opt => opt.MapFrom(src => src.IdImagen))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.UrlImagen));

            // Mapeo de Reseña a ResenaDto
            CreateMap<Reseña, ResenaDto>()
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src =>
                    src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.Correo : string.Empty));

            // Mapeo de CrearProductoDto a Producto
            CreateMap<CrearProductoDto, Producto>()
                .ForMember(dest => dest.IdProducto, opt => opt.Ignore())
                .ForMember(dest => dest.ImagenProducto, opt => opt.Ignore())
                .ForMember(dest => dest.IdEtiqueta, opt => opt.Ignore())
                .ForMember(dest => dest.IdCategoriaNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.CarritoProducto, opt => opt.Ignore())
                .ForMember(dest => dest.PedidoProducto, opt => opt.Ignore())
                .ForMember(dest => dest.ProductoPersonalizado, opt => opt.Ignore())
                .ForMember(dest => dest.Promocion, opt => opt.Ignore())
                .ForMember(dest => dest.Reseña, opt => opt.Ignore());

            // Mapeo de EditarProductoDto a Producto
            CreateMap<EditarProductoDto, Producto>()
                .ForMember(dest => dest.ImagenProducto, opt => opt.Ignore())
                .ForMember(dest => dest.IdEtiqueta, opt => opt.Ignore())
                .ForMember(dest => dest.IdCategoriaNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.CarritoProducto, opt => opt.Ignore())
                .ForMember(dest => dest.PedidoProducto, opt => opt.Ignore())
                .ForMember(dest => dest.ProductoPersonalizado, opt => opt.Ignore())
                .ForMember(dest => dest.Promocion, opt => opt.Ignore())
                .ForMember(dest => dest.Reseña, opt => opt.Ignore());

            // Mapeo de Producto a EditarProductoDto
            CreateMap<Producto, EditarProductoDto>()
                .ForMember(dest => dest.EtiquetasIds, opt => opt.Ignore())
                .ForMember(dest => dest.ImagenesNuevasArchivos, opt => opt.Ignore())
                .ForMember(dest => dest.ImagenesAEliminar, opt => opt.Ignore())
                .ForMember(dest => dest.ImagenesExistentes, opt => opt.MapFrom(src => src.ImagenProducto));

            //.ForMember(dest => dest.ImagenesExistentes, opt => opt.Ignore());
        }

        // Verifica si el producto tiene una promoción activa en la fecha actual
        private static bool TienePromocionActiva(Producto src)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            // Verificar promociones directas del producto
            var tienePromocionProducto = src.Promocion != null && src.Promocion.Any(p =>
                p.FechaInicio <= hoy &&
                p.FechaFin >= hoy &&
                p.PorcentajeDescuento.HasValue &&
                p.PorcentajeDescuento.Value > 0
            );

            // Verificar promociones de la categoría del producto
            var tienePromocionCategoria = src.IdCategoriaNavigation?.Promocion != null && 
                src.IdCategoriaNavigation.Promocion.Any(p =>
                    p.FechaInicio <= hoy &&
                    p.FechaFin >= hoy &&
                    p.PorcentajeDescuento.HasValue &&
                    p.PorcentajeDescuento.Value > 0
                );

            return tienePromocionProducto || tienePromocionCategoria;
        }

        // Calcula el precio final aplicando el descuento de la promoción activa
        private static decimal? CalcularPrecioConDescuento(Producto src)
        {
            if (!src.Precio.HasValue)
                return src.Precio;

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            
            // Buscar promoción directa del producto
            var promoProducto = src.Promocion?
                .FirstOrDefault(p =>
                    p.FechaInicio <= hoy &&
                    p.FechaFin >= hoy &&
                    p.PorcentajeDescuento.HasValue &&
                    p.PorcentajeDescuento.Value > 0
                );

            // Buscar promoción de la categoría del producto
            var promoCategoria = src.IdCategoriaNavigation?.Promocion?
                .FirstOrDefault(p =>
                    p.FechaInicio <= hoy &&
                    p.FechaFin >= hoy &&
                    p.PorcentajeDescuento.HasValue &&
                    p.PorcentajeDescuento.Value > 0
                );

            // Aplicar la promoción con mayor descuento (prioridad: producto > categoría)
            var promocionAplicar = promoProducto ?? promoCategoria;

            return promocionAplicar?.PorcentajeDescuento != null
                ? src.Precio.Value * (1 - promocionAplicar.PorcentajeDescuento.Value / 100)
                : src.Precio;
        }
    }
}