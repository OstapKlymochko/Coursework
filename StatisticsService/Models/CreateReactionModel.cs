namespace StatisticsService.Models
{
	public class CreateReactionModel
	{
		public int SongId { get; set; }
		public string Type { get; set; } = null!;
	}
}
