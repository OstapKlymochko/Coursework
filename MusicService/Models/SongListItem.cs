namespace MusicService.Models
{
	public class SongListItem
	{
		public int Id { get; set; }
		public int AuthorId { get; set; }
		public string Title { get; set; } = null!;
		public string Genre { get; set; } = null!;
		public string LogoFileKey { get; set; } = null!;
	}
}
