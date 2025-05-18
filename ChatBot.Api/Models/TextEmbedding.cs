using System.ComponentModel.DataAnnotations;

namespace ChatBot.Api.Models
{
    public class TextEmbedding
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(4000)]
        public string? Message { get; set; }

        public byte[]? Embedding { get; set; }
    }
}
