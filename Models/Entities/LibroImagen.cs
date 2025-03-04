using System.ComponentModel.DataAnnotations;

namespace hoshibunko.Models.Entities
{
    public class LibroImagen
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LibroId { get; set; }
        public Libro Libro { get; set; } // ✅ Relación con el libro

        [Required]
        public string UrlImagen { get; set; } // ✅ URL de la imagen
    }
}

