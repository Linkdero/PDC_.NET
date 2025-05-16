using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDC.Models;
using PDC.Data;
using Microsoft.AspNetCore.Authorization;

namespace PDC.Controllers
{
    [ApiController]
    [Route("api/paises")]
    [Authorize]
    public class PaisController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaisController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaises()
        {
            var paises = await _context.Paises
                .Select(p => new
                {
                    id_pais = p.IdPais,
                    pais = p.NombrePais
                })
                .ToListAsync();

            return Ok(paises);
        }

        [HttpPost]
        public async Task<IActionResult> setNuevoPais([FromForm] string nombreNuevoPais)
        {
            try
            {
                var nuevoPais = new Pais
                {
                    NombrePais = nombreNuevoPais
                };

                _context.Paises.Add(nuevoPais);
                await _context.SaveChangesAsync();

                return Ok(new { msg = "País registrado correctamente", nuevoPais.IdPais });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Error al guardar el país", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> setEliminarPais(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pais = await _context.Paises
                    .Include(p => p.Departamentos)
                        .ThenInclude(d => d.Municipios)
                            .ThenInclude(m => m.Empresas)
                    .FirstOrDefaultAsync(p => p.IdPais == id);

                if (pais == null)
                    return NotFound(new { msg = "País no encontrado" });

                foreach (var d in pais.Departamentos)
                {
                    foreach (var m in d.Municipios)
                    {
                        foreach (var empresa in m.Empresas)
                        {
                            var detalles = _context.ColaboradorEmpresas
                                .Where(d => d.IdEmpresa == empresa.IdEmpresa);
                            _context.ColaboradorEmpresas.RemoveRange(detalles);
                            _context.Empresas.Remove(empresa);
                        }
                        _context.Municipios.Remove(m);
                    }
                    _context.Departamentos.Remove(d);
                }
                _context.Paises.Remove(pais);

                await _context.SaveChangesAsync();

                var colaboradores = await _context.Colaboradores.ToListAsync();
                foreach (var c in colaboradores)
                {
                    bool tieneEmpresas = await _context.ColaboradorEmpresas.AnyAsync(ce => ce.IdColaborador == c.IdColaborador);
                    if (!tieneEmpresas)
                    {
                        _context.Colaboradores.Remove(c);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { msg = "País eliminado correctamente", id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    msg = "ERROR: " + ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

    }
}