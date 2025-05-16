using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDC.Models
{
    [Table("municipio")]
    public class Municipio
    {
        [Key]
        [Column("id_municipio")]
        public int IdMunicipio { get; set; }

        [Required]
        [Column("municipio")]
        [StringLength(30)]
        public string NombreMunicipio { get; set; }

        [ForeignKey("Departamento")]
        [Column("id_departamento")]
        public int IdDepartamento { get; set; }
        public Departamento Departamento { get; set; }

        public ICollection<Empresa> Empresas { get; set; }
    }
}