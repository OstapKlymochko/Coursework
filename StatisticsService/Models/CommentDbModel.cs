using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatisticsService.Models
{
    [Table("comments")]
    public class CommentDbModel
    {
        public CommentDbModel()
        {
            CreatedAt = DateTime.Now.ToUniversalTime();
            Attached = false;
            Edited = false;
        }
        public CommentDbModel(CreateCommentModel createModel) : this()
        {
            SongId = createModel.SongId;
            Body = createModel.Body;
            RepliedTo = createModel.RepliedTo;
            UserName = createModel.UserName;
        }
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("userid")]
        public int UserId { get; set; }
        [Column("songid")]
        public int SongId { get; set; }
        [Column("username")]
        public string UserName { get; set; } = null!;
        [Column("body")]
        public string Body { get; set; } = null!;
        [Column("createdat")]
        public DateTime CreatedAt { get; set; }
        [Column("attached")]
        public bool Attached { get; set; }
        [Column("edited")]
        public bool Edited { get; set; }
        [Column("repliedto")]
        public int? RepliedTo { get; set; } = null!;
    }
}
