using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jokesWebApp.Models
{
    public class JokeComment
    {
        public int Id { get; set; }

        [Required]
        public required string CommentText { get; set; }

        [Required]
        public int JokeId { get; set; }

        [Required]
        public required string UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("JokeId")]
        public Joke? Joke { get; set; }
    }
}
