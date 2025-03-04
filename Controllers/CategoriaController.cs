using Microsoft.AspNetCore.Mvc;
using hoshibunko.Services.Interfaces;
using hoshibunko.Models.DTOs;

namespace hoshibunko.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAll()
        {
            return Ok(await _categoriaService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDTO>> GetById(int id)
        {
            var categoria = await _categoriaService.GetByIdAsync(id);
            if (categoria == null) return NotFound();
            return Ok(categoria);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CategoriaDTO categoriaDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _categoriaService.CreateAsync(categoriaDto);
            return CreatedAtAction(nameof(GetById), new { id = categoriaDto.Id }, categoriaDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CategoriaDTO categoriaDto)
        {
            var updated = await _categoriaService.UpdateAsync(id, categoriaDto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _categoriaService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}


