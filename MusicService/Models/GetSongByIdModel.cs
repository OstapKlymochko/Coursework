namespace MusicService.Models
{
    public class GetSongByIdModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public string SongFileKey { get; set; } = null!;
        public string LogoFileKey { get; set; } = null!;
        public string? VideoClipFileKey { get; set; }
        public int AuthorId { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Comments { get; set; }
        public bool IsLiked {  get; set; }
        public bool IsDisliked {  get; set; }
    }
}
