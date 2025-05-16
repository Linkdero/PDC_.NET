using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDC.Models;
using PDC.Data;
using Microsoft.AspNetCore.Authorization;

namespace PDC.Controllers
{
    [ApiController]
    [Route("api/colaborador")]
    [Authorize]
    public class ColaboradorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ColaboradorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllColaboradores()
        {
            var colaboradores = await _context.Colaboradores
                .Include(c => c.Empresas)
                .ThenInclude(ce => ce.Empresa)
                .Select(c => new
                {
                    id_colaborador = c.IdColaborador,
                    nombre = c.Nombre,
                    apellido = c.Apellido,
                    nombres_comerciales = string.Join(", ", c.Empresas.Select(e =>
                        $"{e.Empresa.Nit} - {e.Empresa.RazonSocial} - {e.Empresa.NombreComercial}"
                    )),
                    edad = c.Edad,
                    telefono = c.Telefono,
                    correo = c.Correo,
                    fecha_creacion = c.FechaCreacion
                })
                .ToListAsync();

            return Ok(colaboradores);
        }
        [HttpPost]
        public async Task<IActionResult> setNuevoColaborador([FromBody] Colaborador colaborador)
        {
            try
            {
                var nuevoColaborador = new Colaborador
                {
                    Nombre = colaborador.Nombre,
                    Apellido = colaborador.Apellido,
                    Edad = colaborador.Edad,
                    Telefono = colaborador.Telefono,
                    Correo = colaborador.Correo,
                    FechaCreacion = DateTime.Now
                };

                _context.Colaboradores.Add(nuevoColaborador);
                await _context.SaveChangesAsync();

                // Relacionar con empresas (si hay)
                if (colaborador.Empresas != null && colaborador.Empresas.Any())
                {
                    foreach (var emp in colaborador.Empresas)
                    {
                        var relacion = new ColaboradorEmpresa
                        {
                            IdColaborador = nuevoColaborador.IdColaborador,
                            IdEmpresa = emp.IdEmpresa
                        };
                        _context.ColaboradorEmpresas.Add(relacion);
                    }
                    await _context.SaveChangesAsync();
                }

                return Ok(new { msg = "Colaborador registrado correctamente" });
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { msg = "ERROR: " + errorMsg });
            }
        }
        // DELETE: api/colaborador/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> SetEliminarColaborador(int id)
        {
            try
            {
                var colaborador = await _context.Colaboradores.FindAsync(id);

                if (colaborador == null)
                    return NotFound(new { msg = $"No se encontró al colaborador con ID {id}" });

                var detalles = _context.ColaboradorEmpresas.Where(d => d.IdColaborador == id);
                _context.ColaboradorEmpresas.RemoveRange(detalles);

                _context.Colaboradores.Remove(colaborador);
                await _context.SaveChangesAsync();

                return Ok(new { msg = "Colaborador Eliminado", id = id });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "ERROR: " + e.Message });
            }
        }
    }

}
