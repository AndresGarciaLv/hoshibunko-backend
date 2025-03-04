using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using hoshibunko.Models.Entities;

public class Usuario : IdentityUser
{
    [Required]
    public string Nombre { get; set; }

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
}
