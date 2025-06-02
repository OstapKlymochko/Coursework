namespace StatisticsService.Models
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SongId { get; set; }
        public string UserName { get; set; } = null!;
        public string Body { get; set; } = null!;
        public bool Edited { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Attached { get; set; }
        public int RepliesCount { get; set; }
    }
}
