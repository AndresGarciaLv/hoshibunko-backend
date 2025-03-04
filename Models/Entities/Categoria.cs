using System.ComponentModel.DataAnnotations;

namespace hoshibunko.Models.Entities
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public ICollection<LibroCategoria> LibrosCategorias { get; set; } = new List<LibroCategoria>();
    }
}
