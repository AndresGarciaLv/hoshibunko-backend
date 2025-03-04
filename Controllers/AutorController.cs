using Microsoft.AspNetCore.Mvc;
using hoshibunko.Services.Interfaces;
using hoshibunko.Models.DTOs;

namespace hoshibunko.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutorController : ControllerBase
    {
        private readonly IAutorService _autorService;

        public AutorController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> GetAll()
        {
            return Ok(await _autorService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AutorDTO>> GetById(int id)
        {
            var autor = await _autorService.GetByIdAsync(id);
            if (autor == null) return NotFound();
            return Ok(autor);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] AutorDTO autorDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _autorService.CreateAsync(autorDto);
            return CreatedAtAction(nameof(GetById), new { id = autorDto.Id }, autorDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] AutorDTO autorDto)
        {
            var updated = await _autorService.UpdateAsync(id, autorDto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _autorService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}