﻿using Melodix.Models;
using Melodix.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
        
namespace Melodix.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PerfilUsuario> PerfilesUsuario { get; set; }
        public DbSet<Artista> Artistas { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Pista> Pistas { get; set; }
        public DbSet<ListaReproduccion> ListasReproduccion { get; set; }
        public DbSet<ListaPista> ListaPistas { get; set; }
        public DbSet<HistorialEscucha> HistorialesEscucha { get; set; }
        public DbSet<HistorialLike> HistorialesLike { get; set; }
        public DbSet<UsuarioLikeAlbum> UsuarioLikesAlbum { get; set; }
        public DbSet<UsuarioLikeLista> UsuarioLikesLista { get; set; }
        public DbSet<UsuarioLikePista> UsuarioLikesPista { get; set; }
        public DbSet<UsuarioSigue> UsuariosSigue { get; set; }
        public DbSet<UsuarioSigueArtista> UsuariosSigueArtista { get; set; }
        public DbSet<UsuarioSigueLista> UsuariosSigueLista { get; set; }
        public DbSet<PlanSuscripcion> PlanesSuscripcion { get; set; }
        public DbSet<Suscripcion> Suscripciones { get; set; }
        public DbSet<SuscripcionUsuario> SuscripcionesUsuario { get; set; }

        public DbSet<TransaccionPago> TransaccionesPago { get; set; }
        public DbSet<ArchivoSubido> ArchivosSubidos { get; set; }
        public DbSet<LogSistema> LogsSistema { get; set; }
        public DbSet<SolicitudMusico> SolicitudesMusico { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relación 1:1 entre usuario Identity y perfil extendido
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Perfil)
                .WithOne(p => p.Usuario)
                .HasForeignKey<PerfilUsuario>(p => p.UsuarioId);

            // Relaciones UsuarioSigue (seguidores y seguidos)
            builder.Entity<UsuarioSigue>()
                .HasKey(us => new { us.SeguidorId, us.SeguidoId });

            builder.Entity<UsuarioSigue>()
                .HasOne(us => us.Seguidor)
                .WithMany(u => u.Seguidos)
                .HasForeignKey(us => us.SeguidorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsuarioSigue>()
                .HasOne(us => us.Seguido)
                .WithMany(u => u.Seguidores)
                .HasForeignKey(us => us.SeguidoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relaciones para UsuarioSigueArtista
            builder.Entity<UsuarioSigueArtista>()
                .HasOne(usa => usa.Usuario)
                .WithMany(u => u.UsuarioSigueArtistas)
                .HasForeignKey(usa => usa.UsuarioId);

            // Relaciones para UsuarioSigueLista
            builder.Entity<UsuarioSigueLista>()
                .HasOne(usl => usl.Usuario)
                .WithMany(u => u.UsuarioSigueListas)
                .HasForeignKey(usl => usl.UsuarioId);

            // Relaciones para UsuarioLikeAlbum
            builder.Entity<UsuarioLikeAlbum>()
                .HasOne(ula => ula.Usuario)
                .WithMany(u => u.UsuarioLikeAlbums)
                .HasForeignKey(ula => ula.UsuarioId);

            // Relaciones para UsuarioLikeLista
            builder.Entity<UsuarioLikeLista>()
                .HasOne(ull => ull.Usuario)
                .WithMany(u => u.UsuarioLikeListas)
                .HasForeignKey(ull => ull.UsuarioId);

            // Relaciones para UsuarioLikePista
            builder.Entity<UsuarioLikePista>()
                .HasOne(ulp => ulp.Usuario)
                .WithMany(u => u.UsuarioLikePistas)
                .HasForeignKey(ulp => ulp.UsuarioId);

            // Relaciones para HistorialEscucha
            builder.Entity<HistorialEscucha>()
                .HasOne(he => he.Usuario)
                .WithMany(u => u.HistorialEscuchas)
                .HasForeignKey(he => he.UsuarioId);

            // Relaciones para HistorialLike
            builder.Entity<HistorialLike>()
                .HasOne(hl => hl.Usuario)
                .WithMany(u => u.HistorialLikes)
                .HasForeignKey(hl => hl.UsuarioId);

            // Relaciones para SuscripcionUsuario
            builder.Entity<SuscripcionUsuario>()
                .HasOne(su => su.Usuario)
                .WithMany(u => u.SuscripcionUsuarios)
                .HasForeignKey(su => su.UsuarioId);

            builder.Entity<Suscripcion>()
                .HasOne(s => s.Usuario)
                .WithMany(u => u.Suscripciones)
                .HasForeignKey(s => s.UsuarioId);

            // Relaciones para TransaccionPago
            builder.Entity<TransaccionPago>()
                .HasOne(tp => tp.Usuario)
                .WithMany(u => u.TransaccionesPago)
                .HasForeignKey(tp => tp.UsuarioId);

            builder.Entity<TransaccionPago>()
                .HasOne(tp => tp.Suscripcion)
                .WithMany(s => s.TransaccionesPago)
                .HasForeignKey(tp => tp.SuscripcionId);

            // Relaciones para ArchivoSubido
            builder.Entity<ArchivoSubido>()
                .HasOne(a => a.Usuario)
                .WithMany(u => u.ArchivosSubidos)
                .HasForeignKey(a => a.UsuarioId);

            // Relaciones para LogSistema
            builder.Entity<LogSistema>()
                .HasOne(l => l.Usuario)
                .WithMany(u => u.LogsGenerados)
                .HasForeignKey(l => l.UsuarioId)
                .IsRequired(false);

            // Relaciones para SolicitudMusico
            builder.Entity<SolicitudMusico>()
                .HasOne(s => s.Usuario)
                .WithMany(u => u.SolicitudesMusico)
                .HasForeignKey(s => s.UsuarioId);

            builder.Entity<SolicitudMusico>()
                .HasOne(s => s.AdminRevisor)
                .WithMany(u => u.SolicitudesRevisadas)
                .HasForeignKey(s => s.AdminRevisorId)
                .IsRequired(false);

        }


        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is ApplicationUser &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach(var entry in entries)
            {
                var now = DateTime.UtcNow;
                if(entry.State == EntityState.Added)
                {
                    ((ApplicationUser)entry.Entity).CreadoEn = now;
                }
                ((ApplicationUser)entry.Entity).ActualizadoEn = now;
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is ApplicationUser &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach(var entry in entries)
            {
                var now = DateTime.UtcNow;
                if(entry.State == EntityState.Added)
                {
                    ((ApplicationUser)entry.Entity).CreadoEn = now;
                }
                ((ApplicationUser)entry.Entity).ActualizadoEn = now;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}