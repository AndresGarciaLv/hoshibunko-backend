namespace hoshibunko.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Libro
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Required]
    public string Descripcion { get; set; }

    [Required]
    public int AutorId { get; set; }
    public required Autor Autor { get; set; }

    public ICollection<LibroCategoria> LibrosCategorias { get; set; } = new List<LibroCategoria>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();

    public List<LibroImagen> Imagenes { get; set; } = new List<LibroImagen>();
}