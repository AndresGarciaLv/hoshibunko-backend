using hoshibunko.Models.Entities;
using Hoshibunko.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Asegurar que la base de datos esté creada
        await context.Database.EnsureCreatedAsync();

        // 1. Crear roles si no existen
        var roles = new[] { "ADMIN", "USER" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Sembrar usuarios con datos reales
        if (!userManager.Users.Any())
        {
            var users = new List<(string UserName, string Email, string Password, string Role, string Nombre)>
            {
                ("admin", "admin@hoshibunko.com", "Admin123!", "ADMIN", "Administrador"),
                ("jperez", "juan@hoshibunko.com", "User123!", "USER", "Juan Pérez"),
                ("agarcia", "andresgarciia09@gmail.com", "Toluca10!", "admin", "Andrés García"),
                ("mpoot", "mirian@gmail.com", "User123!", "USER", "Mirian Poot"),
                ("anamartinez", "ana.martinez@ejemplo.com", "User123!", "USER", "Ana Martínez")
            };

            foreach (var (userName, email, password, role, nombre) in users)
            {
                var user = new Usuario
                {
                    UserName = userName,
                    Email = email,
                    Nombre = nombre
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        // 3. Sembrar autores con nombres reales
        if (!context.Autores.Any())
        {
            var autores = new List<Autor>
            {
                new() { Nombre = "Gabriel García Márquez" },
                new() { Nombre = "Isabel Allende" },
                new() { Nombre = "Julio Cortázar" },
                new() { Nombre = "Mario Vargas Llosa" },
                new() { Nombre = "Carlos Fuentes" }
            };

            context.Autores.AddRange(autores);
            await context.SaveChangesAsync();
        }

        // 4. Sembrar categorías reales
        if (!context.Categorias.Any())
        {
            var categorias = new List<Categoria>
            {
                new() { Nombre = "Novela" },
                new() { Nombre = "Cuento" },
                new() { Nombre = "Poesía" },
                new() { Nombre = "Ensayo" },
                new() { Nombre = "Teatro" }
            };

            context.Categorias.AddRange(categorias);
            await context.SaveChangesAsync();
        }

        // 5. Sembrar libros con categorías correctamente asignadas
        if (!context.Libros.Any())
        {
            var autores = await context.Autores.ToListAsync();
            var categorias = await context.Categorias.ToListAsync();

            var libros = new List<Libro>
            {
                new() { Nombre = "Cien Años de Soledad", Descripcion = "Una obra maestra de la literatura latinoamericana que narra la historia de la familia Buendía en el mítico Macondo.", Autor = autores.First(a => a.Nombre == "Gabriel García Márquez") },
                new() { Nombre = "La Casa de los Espíritus", Descripcion = "Novela que entrelaza la historia familiar y política en Chile, escrita por la reconocida Isabel Allende.", Autor = autores.First(a => a.Nombre == "Isabel Allende") },
                new() { Nombre = "Rayuela", Descripcion = "Una obra innovadora que rompe con la narrativa tradicional, permitiendo múltiples formas de lectura, de Julio Cortázar.", Autor = autores.First(a => a.Nombre == "Julio Cortázar") },
                new() { Nombre = "La Ciudad y los Perros", Descripcion = "Una cruda representación de la disciplina militar y la sociedad peruana, escrita por Mario Vargas Llosa.", Autor = autores.First(a => a.Nombre == "Mario Vargas Llosa") },
                new() { Nombre = "La Región Más Transparente", Descripcion = "Una exploración de la identidad y el poder en la sociedad mexicana, obra emblemática de Carlos Fuentes.", Autor = autores.First(a => a.Nombre == "Carlos Fuentes") }
            };

            context.Libros.AddRange(libros);
            await context.SaveChangesAsync();

            // Asignar categorías correctamente a cada libro
            foreach (var libro in libros)
            {
                var categoriasAsignadas = new List<LibroCategoria>
                {
                    new() { LibroId = libro.Id, Libro = libro, CategoriaId = categorias.First(c => c.Nombre == "Novela").Id, Categoria = categorias.First(c => c.Nombre == "Novela") },
                    new() { LibroId = libro.Id, Libro = libro, CategoriaId = categorias.First(c => c.Nombre == "Ensayo").Id, Categoria = categorias.First(c => c.Nombre == "Ensayo") }
                };

                context.LibrosCategorias.AddRange(categoriasAsignadas);
            }
            await context.SaveChangesAsync();
        }
    }
}
