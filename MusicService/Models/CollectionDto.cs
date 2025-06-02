namespace MusicService.Models
{
	public class CollectionDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = null!;
		public int SongsCount { get; set; }
		public int OwnerId { get; set; }
		public string OwnerUserName { get; set; } = null!;
		public string ImageUrl { get; set; } = null!;
	}
}
