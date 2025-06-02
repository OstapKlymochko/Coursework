namespace MusicService.Models
{
	public class CreateCollectionModel
	{
		public string Title { get; set; } = null!;
		public string Type { get; set; } = null!;
		public string OwnerUserName { get; set; } = null!;
	}
}
