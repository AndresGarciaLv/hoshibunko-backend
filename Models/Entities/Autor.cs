using System.ComponentModel.DataAnnotations;

namespace hoshibunko.Models.Entities
{
    public class Autor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public ICollection<Libro> Libros { get; set; } = new List<Libro>();
    }
}
