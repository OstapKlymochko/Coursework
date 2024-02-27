using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models
{
	[Table("users")]
	public class UserModel
	{
		[Key]
		[Column("id")]
		public int Id { get; set; }
		[Column("firstname")]
		public string? FirstName { get; set; }
		[Column("lastname")]
		public string? LastName { get; set; }
	}
}
