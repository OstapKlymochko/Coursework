using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilesService.Models
{
	[Table("files")]
	public class FileDbModel
	{
		[Key]
		[Column("id")]
		public int Id { get; set; }
		[Column("filename")]
		public string FileName { get; set; } = null!;
		[Column("filetype")]
		public string FileType { get; set; } = null!;
		[Column("displayFileName")]
		public string DisplayFileName { get; set; } = null!;
	}
}
