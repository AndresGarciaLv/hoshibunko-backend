namespace hoshibunko.Models.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Like
{
    public int Id { get; set; }

    public int LibroId { get; set; }
    public Libro Libro { get; set; } // ✅ Relación correctamente definida

    public string UsuarioId { get; set; }
    public Usuario Usuario { get; set; } // ✅ Relación con usuario
}


