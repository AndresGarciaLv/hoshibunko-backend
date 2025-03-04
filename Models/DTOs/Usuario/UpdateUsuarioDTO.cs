namespace hoshibunko.Models.DTOs.Usuario
{
    public class UpdateUsuarioDTO
    {
        public string? Id { get; set; }
        public string Nombre { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public List<string> Roles { get; set; }
    }
}
