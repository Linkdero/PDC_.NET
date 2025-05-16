using Microsoft.EntityFrameworkCore;
using PDC.Models;

namespace PDC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Pais> Paises { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<ColaboradorEmpresa> ColaboradorEmpresas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración para que no intente pluralizar los nombres de las tablas
            modelBuilder.Entity<Pais>().ToTable("pais");
            modelBuilder.Entity<Departamento>().ToTable("departamento");
            modelBuilder.Entity<Municipio>().ToTable("municipio");
            modelBuilder.Entity<Empresa>().ToTable("tbl_empresa");
            modelBuilder.Entity<Colaborador>().ToTable("tbl_colaborador");
            modelBuilder.Entity<ColaboradorEmpresa>().ToTable("tbl_colaborador_detalle_empresa");

            // Configuración de la relación muchos a muchos
            modelBuilder.Entity<ColaboradorEmpresa>()
                .HasOne(ce => ce.Colaborador)
                .WithMany(c => c.Empresas)
                .HasForeignKey(ce => ce.IdColaborador)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ColaboradorEmpresa>()
                .HasOne(ce => ce.Empresa)
                .WithMany(e => e.Colaboradores)
                .HasForeignKey(ce => ce.IdEmpresa)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}