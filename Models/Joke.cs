using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jokesWebApp.Models
{
    public class Joke
    {
        public int Id { get; set; }

        [Required]
        public required string JokeQuestion { get; set; }

        [Required]
        public required string JokeAnswer { get; set; }

        public string? UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<JokeLike> Likes { get; set; } = new List<JokeLike>();
        public ICollection<JokeComment> Comments { get; set; } = new List<JokeComment>();
    }
}
