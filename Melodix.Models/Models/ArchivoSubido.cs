using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melodix.Models.Models
{
    // 2. Archivos subidos (música, imágenes, otros)
    public class ArchivoSubido
    {
        // Necesarios
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public TipoArchivo Tipo { get; set; }
        public string NombreArchivo { get; set; }
        public string RutaAlmacenamiento { get; set; }
        public long TamanoBytes { get; set; }
        public DateTime FechaSubida { get; set; }
        public bool EsPublico { get; set; }
        public string? Extension { get; set; }
        public string? HashArchivo { get; set; } // Para evitar duplicados
        public string? Descripcion { get; set; }
        public string? EntidadReferencia { get; set; } // Ej: "Pista", "PerfilUsuario"
        public int? IdReferencia { get; set; } // ID de la pista o entidad

        // Navegadores
        public ApplicationUser Usuario { get; set; }
    }
}
