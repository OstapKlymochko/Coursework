using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicService.Models
{
	[Table("genres")]
	public class GenreDbModel
	{
		[Key]
		[Column("id")]
		public int Id { get; set; }

		[Column("title")]
		public string Title { get; set; } = null!;
	}
}
