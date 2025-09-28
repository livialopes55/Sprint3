using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MottuApi.Models
{
    public class Patio
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Descricao { get; set; } = string.Empty;

        [StringLength(60)]
        public string? Dimensao { get; set; }

        public int FilialId { get; set; }

        [JsonIgnore]
        public Filial? Filial { get; set; }

        public ICollection<Moto> Motos { get; set; } = new List<Moto>();
    }
}