namespace StatisticsService.Models
{
    public class CommentsListModel
    {
        public IEnumerable<CommentDto> Comments { get; set; } = null!;
        public int PagesCount { get; set; }
    }
}
