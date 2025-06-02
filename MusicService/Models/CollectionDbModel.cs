using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicService.Models
{
	[Table("collections")]
	public class CollectionDbModel
	{
		[Key]
		[Column("id")]
		public int Id { get; set; }
		[Column("ownerid")]
		public int OwnerId { get; set; }
		[Column("ownerusername")]
		public string OwnerUsername { get; set; } = null!;
		[Column("title")]
		public string Title { get; set; } = null!;
		[Column("type")]
		public string Type { get; set; } = null!;
		[Column("imagefilekey")]
		public string ImageFileKey { get; set; } = null!;
	}
}
