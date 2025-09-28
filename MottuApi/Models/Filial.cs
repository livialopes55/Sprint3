using System.ComponentModel.DataAnnotations;

namespace MottuApi.Models
{
    public class Filial
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Nome { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Endereco { get; set; } = string.Empty;

        public ICollection<Patio> Patios { get; set; } = new List<Patio>();
    }
}