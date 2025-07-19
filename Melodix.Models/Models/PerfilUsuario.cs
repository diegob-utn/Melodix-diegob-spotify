using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melodix.Models.Models
{
    // Información extendida de usuario (perfil)
    public class PerfilUsuario
    {
        // Necesarios
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string? Proveedor { get; set; }
        public string? Biografia { get; set; }
        public string? FotoPerfil { get; set; }
        public string? Ubicacion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? SpotifyId { get; set; }
        public string? SpotifyTokenAcceso { get; set; }
        public string? SpotifyTokenRefresco { get; set; }

        // Navegadores
        public ApplicationUser Usuario { get; set; }
    }
}
