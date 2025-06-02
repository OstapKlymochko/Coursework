using System.Text.Json.Serialization;

namespace MusicService.Models
{
    public class SongDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public int AuthorId { get; set; }
        public string AuthorPseudonym { get; set; } = null!;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public double? Rank { get; set; }
    }
}
