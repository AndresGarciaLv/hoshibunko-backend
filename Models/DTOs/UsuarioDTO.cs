namespace hoshibunko.Models.DTOs
{
    public class UsuarioDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string AspNetUserId { get; set; }
        public List<string> Roles { get; set; }
    }
}

