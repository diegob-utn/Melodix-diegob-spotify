    using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace Melodix.Models.Models
{
    // Usuario Identity + perfil extendido
    // Usuario Identity + perfil extendido
    public class ApplicationUser:IdentityUser
    {
        // Necesarios

        [Required(ErrorMessage = "El campo Nick es obligatorio.")]
        public string Nick { get; set; }
        public RolUsuario? Rol { get; set; }
        public bool? Activo { get; set; }
        public DateTime? CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }

        // Navegadores
        public PerfilUsuario? Perfil { get; set; }
        public List<ListaReproduccion> ListasReproduccion { get; set; } = new();
        public List<HistorialEscucha> HistorialEscuchas { get; set; } = new();
        public List<UsuarioSigue> Seguidos { get; set; } = new();
        public List<UsuarioSigue> Seguidores { get; set; } = new();
        public List<UsuarioLikeAlbum> UsuarioLikeAlbums { get; set; } = new();
        public List<UsuarioLikeLista> UsuarioLikeListas { get; set; } = new();
        public List<UsuarioLikePista> UsuarioLikePistas { get; set; } = new();
        public List<UsuarioSigueArtista> UsuarioSigueArtistas { get; set; } = new();
        public List<UsuarioSigueLista> UsuarioSigueListas { get; set; } = new();
        public List<HistorialLike> HistorialLikes { get; set; } = new();
        public List<SuscripcionUsuario> SuscripcionUsuarios { get; set; } = new();
        public List<Suscripcion> Suscripciones { get; set; } = new();
        public List<ArchivoSubido> ArchivosSubidos { get; set; } = new();
        public List<TransaccionPago> TransaccionesPago { get; set; } = new();
        public List<LogSistema> LogsGenerados { get; set; } = new();
        public List<SolicitudMusico> SolicitudesMusico { get; set; } = new();
        public List<SolicitudMusico> SolicitudesRevisadas { get; set; } = new();
    }
}
