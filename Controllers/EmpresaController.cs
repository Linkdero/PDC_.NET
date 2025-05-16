using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDC.Data;
using PDC.Models;
using Microsoft.AspNetCore.Authorization;

namespace PDC.Controllers
{
    [ApiController]
    [Route("api/empresa")]
    [Authorize]
    public class EmpresaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpresaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmpresas([FromQuery] int opcion)
        {
            var empresas = await _context.Empresas
                .Include(e => e.Municipio)
                    .ThenInclude(m => m.Departamento)
                        .ThenInclude(d => d.Pais)
                .Select(e => new
                {
                    id_empresa = e.IdEmpresa,
                    ubicacion = e.Municipio.Departamento.Pais.NombrePais + " - " +
                                e.Municipio.Departamento.NombreDepartamento + " - " +
                                e.Municipio.NombreMunicipio,
                    nit = e.Nit,
                    razon_social = e.RazonSocial,
                    nombre_comercial = e.NombreComercial,
                    telefono = e.Telefono,
                    correo_electronico = e.CorreoElectronico,
                    fecha_creacion = e.FechaCreacion.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync();

            return Ok(empresas);
        }


        [HttpPost]
        public async Task<IActionResult> setNuevaEmpresa([FromBody] Empresa empresa)
        {
            try
            {
                var nuevaEmpresa = new Empresa
                {
                    IdMunicipio = empresa.IdMunicipio,
                    Nit = empresa.Nit,
                    RazonSocial = empresa.RazonSocial,
                    NombreComercial = empresa.NombreComercial,
                    Telefono = empresa.Telefono,
                    CorreoElectronico = empresa.CorreoElectronico,
                    FechaCreacion = DateTime.Now
                };

                _context.Empresas.Add(nuevaEmpresa);
                await _context.SaveChangesAsync();

                return Ok(new { msg = "Empresa registrada correctamente", nuevaEmpresa.IdEmpresa });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Error al guardar la empresa", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> setEliminarEmpresa(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var empresa = await _context.Empresas.FindAsync(id);

                if (empresa == null)
                    return NotFound(new { msg = $"No se encontró la empresa con ID {id}" });

                var relaciones = _context.ColaboradorEmpresas.Where(ce => ce.IdEmpresa == empresa.IdEmpresa);
                _context.ColaboradorEmpresas.RemoveRange(relaciones);

                _context.Empresas.Remove(empresa);

                await _context.SaveChangesAsync();

                var colaboradores = await _context.Colaboradores.ToListAsync();
                foreach (var colaborador in colaboradores)
                {
                    bool tieneEmpresas = await _context.ColaboradorEmpresas
                        .AnyAsync(ce => ce.IdColaborador == colaborador.IdColaborador);

                    if (!tieneEmpresas)
                        _context.Colaboradores.Remove(colaborador);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { msg = $"Empresa con ID {id} eliminada correctamente" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    msg = "No se pudo eliminar la empresa",
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

    }
}
