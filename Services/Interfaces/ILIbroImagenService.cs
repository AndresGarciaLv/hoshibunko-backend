using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace hoshibunko.Services.Interfaces
{
    public interface ILibroImagenService
    {
        Task<string> SubirImagenAsync(int libroId, IFormFile file);
        Task<string> ObtenerImagenAsync(int libroId);
        Task<bool> EliminarImagenAsync(int libroId);
    }
}


