using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDC.Models
{
    [Table("pais")]
    public class Pais
    {
        [Key]
        [Column("id_pais")]
        public int IdPais { get; set; }

        [Required]
        [Column("pais")]
        [StringLength(50)]
        public string NombrePais { get; set; }

        public ICollection<Departamento> Departamentos { get; set; }
    }
}