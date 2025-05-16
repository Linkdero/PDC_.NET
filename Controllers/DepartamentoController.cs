using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDC.Data;
using PDC.Models;
using Microsoft.AspNetCore.Authorization;

namespace PDC.Controllers
{
    [ApiController]
    [Route("api/departamento")]
    [Authorize]
    public class DepartamentoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartamentoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartamentos([FromQuery] int pais)
        {
            var departamentos = await _context.Departamentos
                .Where(d => d.IdPais == pais)
                .Select(d => new
                {
                    id_departamento = d.IdDepartamento,
                    departamento = d.NombreDepartamento
                })
                .ToListAsync();

            return Ok(departamentos);
        }

        [HttpPost]
        public async Task<IActionResult> setNuevoDepartamento([FromForm] string nombreNuevoDepartamento, [FromForm] int idPais)
        {
            try
            {
                var nuevoDepartamento = new Departamento
                {
                    NombreDepartamento = nombreNuevoDepartamento,
                    IdPais = idPais
                };

                _context.Departamentos.Add(nuevoDepartamento);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    msg = "Departamento creado correctamente",
                    id = nuevoDepartamento.IdDepartamento
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Error al crear el departamento", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> setEliminarDepartamento(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Eliminar municipios primero
                var municipios = _context.Municipios.Where(m => m.IdDepartamento == id);
                _context.Municipios.RemoveRange(municipios);

                // Buscar y eliminar el departamento
                var departamento = await _context.Departamentos.FindAsync(id);
                if (departamento == null)
                    return NotFound(new { msg = "Departamento no encontrado" });

                _context.Departamentos.Remove(departamento);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { msg = "Departamento Eliminado", id = 1 });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { msg = "ERROR: " + ex.Message });
            }
        }
    }
}
