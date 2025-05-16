using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDC.Models
{
    [Table("departamento")]
    public class Departamento
    {
        [Key]
        [Column("id_departamento")]
        public int IdDepartamento { get; set; }

        [Required]
        [Column("departamento")]
        [StringLength(30)]
        public string NombreDepartamento { get; set; }

        [ForeignKey("Pais")]
        [Column("id_pais")]
        public int IdPais { get; set; }
        public Pais Pais { get; set; }

        public ICollection<Municipio> Municipios { get; set; }
    }
}