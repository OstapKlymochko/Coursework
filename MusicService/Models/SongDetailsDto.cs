namespace MusicService.Models
{
    public class SongDetailsDto : SongDto
    {
        public string SongUrl { get; set; } = null!;
        public string? VideoClipUrl { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Comments { get; set; }
        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
        //todo add statistics data (views)
    }
}
