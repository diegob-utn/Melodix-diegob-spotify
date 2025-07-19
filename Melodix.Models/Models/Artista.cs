namespace Melodix.Models
{
    // Música, álbumes, artistas, listas y pistas
    public class Artista
    {
        // Necesarios
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string SpotifyArtistaId { get; set; }
        public string UrlImagen { get; set; }

        // Navegadores
        public List<Pista> Pistas { get; set; } = new();
        public List<Album> Albumes { get; set; } = new();
        public List<UsuarioSigueArtista> UsuariosQueSiguen { get; set; } = new();
    }
}