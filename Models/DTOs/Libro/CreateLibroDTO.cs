namespace hoshibunko.Models.DTOs.Libro
{
    public class CreateLibroDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int AutorId { get; set; }
        public List<int> CategoriaIds { get; set; } = new List<int>();
    }
}
