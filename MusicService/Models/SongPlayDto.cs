namespace MusicService.Models
{
    public class SongPlayDto
    {
        public int UserId { get; set; }
        public int SongId { get; set; }
        public double ListenTime { get; set; }
    }
}
