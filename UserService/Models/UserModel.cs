using LanguageExt.Pipes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models
{
    [Table("users")]
    public class UserModel
    {
        public UserModel()
        {
        }

        public UserModel(UpdateUserModel user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            UserName = user.UserName;  
        }

        public void UpdateUser(UpdateUserModel updateUser)
        {
            FirstName = updateUser.FirstName;
            LastName = updateUser.LastName;
            Email = updateUser.Email;
            UserName = updateUser.UserName;
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("firstname")]
        public string? FirstName { get; set; }
        [Column("lastname")]
        public string? LastName { get; set; }
        [Column("username")]
        public string? UserName { get; set; }
        [Column("email")]
        public string? Email { get; set; }
        [Column("avatarFileKey")]
        public string? AvatarFileKey { get; set; }
    }
}
