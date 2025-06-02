namespace MusicService.Models
{
	public class SongsListDto
	{
		public IEnumerable<SongDto> Songs { get; set; } = null!;
		public int PagesCount { get; set; }
	}
}
