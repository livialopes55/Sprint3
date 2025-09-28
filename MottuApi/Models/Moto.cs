using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MottuApi.Models
{
    public class Moto
    {
        public int Id { get; set; }

        [Required, StringLength(8)]
        public string Placa { get; set; } = string.Empty;

        [Required, StringLength(80)]
        public string Modelo { get; set; } = string.Empty;

        public int Ano { get; set; }

        [StringLength(40)]
        public string? Status { get; set; }

        public int PatioId { get; set; }

        [JsonIgnore]
        public Patio? Patio { get; set; }
    }
}