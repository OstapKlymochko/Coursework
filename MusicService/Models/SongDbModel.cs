using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicService.Models
{
    [Table("songs")]
    public class SongDbModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("title")]
        public string Title { get; set; } = null!;
        [Column("authorId")]
        public int AuthorId { get; set; }
        [Column("songfilekey")]
        public string SongFileKey { get; set; } = null!;
        [Column("videoclipfilekey")]
        public string? VideoClipFileKey { get; set; }
        [Column("logofilekey")]
        public string? LogoFileKey { get; set; } = "default_logo.png";
        [Column("genreId")]
        public int GenreId { get; set; }
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("author_pseudonym")]
        public string AuthorPseudonym { get; set; } = null!;
    }
}
