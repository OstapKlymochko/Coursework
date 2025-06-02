namespace StatisticsService.Models
{
    public class CreateCommentModel
    {
        public int SongId { get; set; }
        public string Body { get; set; } = null!;
        public int? RepliedTo { get; set; } = null!;
        public string UserName { get; set; } = null!;
    }
}
