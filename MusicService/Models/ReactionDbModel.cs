using System.ComponentModel.DataAnnotations.Schema;

namespace MusicService.Models
{
	[Table("song_reactions")]
	public class ReactionDbModel
	{
		[Column("userId")]
		public int UserId { get; set; }
		[Column("songId")]
		public int SongId { get; set; }
		[Column("type")]
		public string Type { get; set; } = null!;
	}
}
