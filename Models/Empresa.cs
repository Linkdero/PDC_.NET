using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace PDC.Models
{
    [Table("tbl_empresa")]
    public class Empresa
    {
        [Key]
        [Column("id_empresa")]
        public int IdEmpresa { get; set; }

        [ForeignKey("Municipio")]
        [Column("id_municipio")]
        public int IdMunicipio { get; set; }
        [JsonIgnore]
        public Municipio? Municipio { get; set; } 
        [Required]
        
        [Column("nit")]
        [StringLength(20)]
        public string Nit { get; set; }

        [Required]
        [Column("razon_social")]
        [StringLength(150)]
        public string RazonSocial { get; set; }

        [Column("nombre_comercial")]
        [StringLength(150)]
        public string NombreComercial { get; set; }

        [Column("telefono")]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Column("correo_electronico")]
        [StringLength(100)]
        public string CorreoElectronico { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [JsonIgnore]
        public ICollection<ColaboradorEmpresa>? Colaboradores { get; set; }  // Hacerla nullable
    }
}