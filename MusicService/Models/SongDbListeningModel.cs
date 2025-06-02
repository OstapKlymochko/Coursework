using System.ComponentModel.DataAnnotations.Schema;

namespace MusicService.Models
{
    [Table("song_listening")]
    public class SongDbListeningModel
    {
        [Column("userId")]
        public int UserId { get; set; }
        [Column("songId")]
        public int SongId { get; set; }
        [Column("timestamp")]
        public DateTime ListenedAt { get; set; } = DateTime.UtcNow;
        [Column("listenTime")]
        public uint ListenTime { get; set; }
    }
}
