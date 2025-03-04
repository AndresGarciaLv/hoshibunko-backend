using System.ComponentModel.DataAnnotations;

namespace hoshibunko.Models.Entities
{
    public class LibroCategoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LibroId { get; set; }
        public required Libro Libro { get; set; }

        [Required]
        public int CategoriaId { get; set; }
        public required Categoria Categoria { get; set; }
    }
}
