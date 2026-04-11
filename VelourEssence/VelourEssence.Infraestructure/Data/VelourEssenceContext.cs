using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VelourEssence.Infraestructure.Models;

namespace VelourEssence.Infraestructure.Data;

public partial class VelourEssenceContext : DbContext
{
    public VelourEssenceContext(DbContextOptions<VelourEssenceContext> options)
        : base(options)
    {
    }



    public virtual DbSet<Carrito> Carrito { get; set; }

    public virtual DbSet<CarritoProducto> CarritoProducto { get; set; }

    public virtual DbSet<CarritoProductoPersonalizado> CarritoProductoPersonalizado { get; set; }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<CriterioPersonalizacion> CriteriosPersonalizacion { get; set; }

    public virtual DbSet<EstadoPedido> EstadoPedido { get; set; }

    public virtual DbSet<Etiqueta> Etiqueta { get; set; }

    public virtual DbSet<ImagenProducto> ImagenProducto { get; set; }

    public virtual DbSet<OpcionPersonalizacion> OpcionesPersonalizacion { get; set; }

    public virtual DbSet<Pedido> Pedido { get; set; }

    public virtual DbSet<PedidoProducto> PedidoProducto { get; set; }

    public virtual DbSet<Pago> Pago { get; set; }

    public virtual DbSet<Producto> Producto { get; set; }

    public virtual DbSet<ProductoPersonalizado> ProductoPersonalizado { get; set; }

    public virtual DbSet<ProductoPersonalizadoDetalle> ProductoPersonalizadoDetalles { get; set; }

    public virtual DbSet<Promocion> Promocion { get; set; }

    public virtual DbSet<Reseña> Reseña { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.IdCarrito).HasName("PK__Carrito__8B4A618C45A7EB76");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Carrito)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Carrito__IdUsuar__3C69FB99");
        });

        modelBuilder.Entity<CarritoProducto>(entity =>
        {
            entity.HasKey(e => new { e.IdCarrito, e.IdProducto }).HasName("PK__CarritoP__9BD2E8AD71965EB4");

            entity.HasOne(d => d.IdCarritoNavigation).WithMany(p => p.CarritoProducto)
                .HasForeignKey(d => d.IdCarrito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CarritoPr__IdCar__412EB0B6");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.CarritoProducto)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CarritoPr__IdPro__4222D4EF");
        });

        modelBuilder.Entity<CarritoProductoPersonalizado>(entity =>
        {
            entity.HasKey(e => new { e.IdCarrito, e.IdProductoPersonalizado }).HasName("PK__CarritoP__AFC5164E09C559CC");

            entity.HasOne(d => d.IdCarritoNavigation).WithMany(p => p.CarritoProductoPersonalizado)
                .HasForeignKey(d => d.IdCarrito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CarritoPr__IdCar__44FF419A");

            entity.HasOne(d => d.IdProductoPersonalizadoNavigation).WithMany(p => p.CarritoProductoPersonalizado)
                .HasForeignKey(d => d.IdProductoPersonalizado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CarritoPr__IdPro__45F365D3");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__A3C02A10ED283C6D");

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<EstadoPedido>(entity =>
        {
            entity.HasKey(e => e.IdEstadoPedido).HasName("PK__EstadoPe__86B98371275F5822");

            entity.Property(e => e.NombreEstado).HasMaxLength(50);
        });

        modelBuilder.Entity<Etiqueta>(entity =>
        {
            entity.HasKey(e => e.IdEtiqueta).HasName("PK__Etiqueta__5041D723CE1AB381");

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<ImagenProducto>(entity =>
        {
            entity.HasKey(e => e.IdImagen).HasName("PK__ImagenPr__B42D8F2AD26D1EBA");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ImagenProducto)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK__ImagenPro__IdPro__2F10007B");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.IdPedido).HasName("PK__Pedido__9D335DC3E899E9E9");

            entity.Property(e => e.DireccionEnvio).HasMaxLength(200);
            entity.Property(e => e.FechaPedido)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuesto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MetodoPago).HasMaxLength(50);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdEstadoPedidoNavigation).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.IdEstadoPedido)
                .HasConstraintName("FK__Pedido__IdEstado__4CA06362");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Pedido__IdUsuari__4AB81AF0");
        });

        modelBuilder.Entity<PedidoProducto>(entity =>
        {
            entity.HasKey(e => new { e.IdPedido, e.IdProducto }).HasName("PK__PedidoPr__8DABD4E271B8A565");

            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.PedidoProducto)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidoPro__IdPed__4F7CD00D");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.PedidoProducto)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidoPro__IdPro__5070F446");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.ToTable("Pagos"); // Mapear a la tabla "Pagos"
            entity.HasKey(e => e.IdPago).HasName("PK__Pago__FC851A3A7E5A9654");

            entity.Property(e => e.TipoPago)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .IsRequired();

            entity.Property(e => e.FechaPago)
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(e => e.EstadoPago)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.NumeroTarjetaEnmascarado)
                .HasMaxLength(4);

            entity.Property(e => e.NombreTitular)
                .HasMaxLength(100);

            entity.Property(e => e.FechaExpiracion)
                .HasMaxLength(5);

            entity.Property(e => e.MontoRecibido)
                .HasColumnType("decimal(10, 2)");

            entity.Property(e => e.Vuelto)
                .HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pago__IdPedido__5534567C");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__098892105564441A");

            entity.Property(e => e.Concentracion).HasMaxLength(50);
            entity.Property(e => e.Genero).HasMaxLength(20);
            entity.Property(e => e.Marca).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Producto)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK__Producto__IdCate__2C3393D0");

            entity.HasMany(d => d.IdEtiqueta).WithMany(p => p.IdProducto)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductoEtiqueta",
                    r => r.HasOne<Etiqueta>().WithMany()
                        .HasForeignKey("IdEtiqueta")
                         //.OnDelete(DeleteBehavior.ClientSetNull)
                         .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK__ProductoE__IdEti__398D8EEE"),
                    l => l.HasOne<Producto>().WithMany()
                        .HasForeignKey("IdProducto")
                        //.OnDelete(DeleteBehavior.ClientSetNull)
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK__ProductoE__IdPro__38996AB5"),
                    j =>
                    {
                        j.HasKey("IdProducto", "IdEtiqueta").HasName("PK__Producto__2C8C8F620171AAD3");
                    });
        });

        modelBuilder.Entity<ProductoPersonalizado>(entity =>
        {
            entity.HasKey(e => e.IdProductoPersonalizado).HasName("PK__Producto__48F77C23E95940B2");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PrecioFinal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.BaseProducto).WithMany(p => p.ProductoPersonalizado)
                .HasForeignKey(d => d.IdProductoBase)
                .HasConstraintName("FK__ProductoP__BaseP__32E0915F");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ProductoPersonalizado)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__ProductoP__IdUsu__31EC6D26");
        });

        modelBuilder.Entity<Promocion>(entity =>
        {
            entity.HasKey(e => e.IdPromocion).HasName("PK__Promocio__35F6C2A580C8FF4A");

            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PorcentajeDescuento).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Tipo).HasMaxLength(20);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Promocion)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK_Promocion_Categoria");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Promocion)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_Promocion_Producto");
        });

        modelBuilder.Entity<Reseña>(entity =>
        {
            entity.HasKey(e => e.IdReseña).HasName("PK__Reseña__A5376EA622AC1C96");

            entity.HasIndex(e => new { e.IdUsuario, e.IdProducto }, "UQ__Reseña__4BFD36B779AA405A").IsUnique();

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Reseña)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK__Reseña__IdProduc__5535A963");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Reseña)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Reseña__IdUsuari__5441852A");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__2A49584CFE50E5F6");

            entity.Property(e => e.NombreRol).HasMaxLength(50);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF976AA1F6F1");

            entity.HasIndex(e => e.Correo, "UQ__Usuario__60695A1915B40EEE").IsUnique();

            entity.Property(e => e.Contraseña).HasMaxLength(200);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.NombreUsuario).HasMaxLength(100);
            entity.Property(e => e.UltimoInicioSesion).HasColumnType("datetime");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Usuario__IdRol__276EDEB3");
        });

        // Configuración de CriterioPersonalizacion
        modelBuilder.Entity<CriterioPersonalizacion>(entity =>
        {
            entity.ToTable("CriterioPersonalizacion");
            entity.HasKey(e => e.IdCriterio);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.TipoDato).HasMaxLength(50).HasDefaultValue("Lista");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Obligatorio).HasDefaultValue(false);
            entity.Property(e => e.Orden).HasDefaultValue(0);
            
            // Relación con Producto
            entity.HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de OpcionPersonalizacion
        modelBuilder.Entity<OpcionPersonalizacion>(entity =>
        {
            entity.ToTable("OpcionPersonalizacion");
            entity.HasKey(e => e.IdOpcion);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Valor).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Etiqueta).HasMaxLength(100);
            entity.Property(e => e.PrecioAdicional).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Orden).HasDefaultValue(0);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.IdCriterioNavigation)
                .WithMany(p => p.Opciones)
                .HasForeignKey(d => d.IdCriterio)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de ProductoPersonalizadoDetalle
        modelBuilder.Entity<ProductoPersonalizadoDetalle>(entity =>
        {
            entity.HasKey(e => e.IdDetalle);
            entity.Property(e => e.PrecioOpcion).HasColumnType("decimal(10,2)");
            entity.Property(e => e.FechaSeleccion).HasDefaultValueSql("(getdate())").HasColumnType("datetime");

            entity.HasOne(d => d.IdProductoPersonalizadoNavigation)
                .WithMany(p => p.ProductoPersonalizadoDetalles)
                .HasForeignKey(d => d.IdProductoPersonalizado)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdCriterioNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdCriterio)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdOpcionNavigation)
                .WithMany(p => p.ProductoPersonalizadoDetalles)
                .HasForeignKey(d => d.IdOpcion)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
