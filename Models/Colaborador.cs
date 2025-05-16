using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDC.Models
{
    [Table("tbl_colaborador")]
    public class Colaborador
    {
        [Key]
        [Column("id_colaborador")]
        public int IdColaborador { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [Column("apellido")]
        [StringLength(100)]
        public string Apellido { get; set; }

        [Column("edad")]
        public int Edad { get; set; }

        [Column("telefono")]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Column("correo")]
        [StringLength(150)]
        public string Correo { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public ICollection<ColaboradorEmpresa> Empresas { get; set; }

        [NotMapped]
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}