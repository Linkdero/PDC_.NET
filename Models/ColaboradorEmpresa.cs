using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PDC.Models
{
    [Table("tbl_colaborador_detalle_empresa")]
    public class ColaboradorEmpresa
    {
        [Key]
        [Column("id_detalle")]
        public int IdDetalle { get; set; }

        [ForeignKey("Colaborador")]
        [Column("id_colaborador")]
        public int IdColaborador { get; set; }
        [JsonIgnore]
        public Colaborador? Colaborador { get; set; }
        [ForeignKey("Empresa")]
        [Column("id_empresa")]
        public int IdEmpresa { get; set; }
        [JsonIgnore]
        public Empresa? Empresa { get; set; }
    }
}