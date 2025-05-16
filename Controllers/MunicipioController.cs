using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDC.Data;
using PDC.Models;
using Microsoft.AspNetCore.Authorization;

namespace PDC.Controllers
{
    [ApiController]
    [Route("api/municipio")]
    [Authorize]

    public class MunicipioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MunicipioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMunicipios([FromQuery] int departamento)
        {
            var municipios = await _context.Municipios
                .Where(m => m.IdDepartamento == departamento)
                .Select(m => new
                {
                    id_municipio = m.IdMunicipio,
                    municipio = m.NombreMunicipio
                })
                .ToListAsync();

            return Ok(municipios);
        }

        [HttpPost]
        public async Task<IActionResult> setNuevoMunicipio([FromForm] string nombreNuevoMunicipio, [FromForm] int idDepartamento)
        {
            try
            {
                var nuevoMunicipio = new Municipio
                {
                    NombreMunicipio = nombreNuevoMunicipio,
                    IdDepartamento = idDepartamento
                };

                _context.Municipios.Add(nuevoMunicipio);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    msg = "Municipio creado correctamente",
                    id = nuevoMunicipio.IdMunicipio
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Error al crear el municipio", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> setEliminarMunicipio(int id)
        {
            try
            {
                var municipio = await _context.Municipios.FindAsync(id);
                if (municipio == null)
                    return NotFound(new { msg = "Municipio no encontrado" });

                _context.Municipios.Remove(municipio);
                await _context.SaveChangesAsync();

                return Ok(new { msg = "Municipio Eliminado", id = 1 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "ERROR: " + ex.Message });
            }
        }
    }
}
