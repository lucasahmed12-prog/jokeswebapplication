using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jokesWebApp.Models
{
    public class JokeLike
    {
        public int Id { get; set; }

        [Required]
        public int JokeId { get; set; }

        [Required]
        public required string UserId { get; set; }

        public DateTime LikedAt { get; set; } = DateTime.UtcNow;

        // Foreign key relationship
        [ForeignKey("JokeId")]
        public Joke? Joke { get; set; }
    }
}
