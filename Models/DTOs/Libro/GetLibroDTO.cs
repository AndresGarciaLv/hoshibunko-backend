namespace hoshibunko.Models.DTOs.Libro
{
    public class GetLibroDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int AutorId { get; set; }
        public string AutorNombre { get; set; } // Nombre del autor
        public List<int> CategoriaIds { get; set; } = new List<int>(); // Lista de IDs de categorías
        public List<string> Categorias { get; set; } = new List<string>(); // Lista de nombres de categorías
        public string UrlImagen { get; set; } // ✅ Nueva propiedad para la imagen
    }
}
