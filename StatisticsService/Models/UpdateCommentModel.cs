namespace StatisticsService.Models
{
	public class UpdateCommentModel
	{
		public string Body { get; set; } = null!;
		public int CommentId { get; set; }
	}
}
