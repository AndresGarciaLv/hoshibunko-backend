using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using hoshibunko.Services.Interfaces;
using hoshibunko.Models.DTOs;

namespace hoshibunko.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Solo usuarios autenticados pueden dar likes
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService;

        public LikeController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetAll()
        {
            return Ok(await _likeService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LikeDTO>> GetById(int id)
        {
            var like = await _likeService.GetByIdAsync(id);
            if (like == null) return NotFound();
            return Ok(like);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] LikeDTO likeDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _likeService.CreateAsync(likeDto);
            return CreatedAtAction(nameof(GetById), new { id = likeDto.Id }, likeDto);
        }

        [HttpDelete("{id}")]
        [Authorize] // Solo el usuario que dio like puede eliminarlo
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _likeService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("libro/{libroId}")]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetLikesByLibroId(int libroId)
        {
            var likes = await _likeService.GetLikesByLibroIdAsync(libroId);
            return Ok(likes);
        }

        [HttpGet("libro/{libroId}/count")]
        public async Task<ActionResult<int>> GetTotalLikesByLibroId(int libroId)
        {
            var totalLikes = await _likeService.GetTotalLikesByLibroIdAsync(libroId);
            return Ok(totalLikes);
        }
    }
}