using FilesService.Attributes;
using System.ComponentModel.DataAnnotations;

namespace FilesService.Models
{
    public class UploadAvatarModel
    {
        [Required(ErrorMessage = "User id must be specified")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Avatar must be attached")]
        [MaxFileSize(1024 * 1024 * 1, ErrorMessage = "File is too big")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Unsupported extension (jpg, jpeg, png allowed)")]
        public IFormFile Avatar { get; set; } = null!;
    }
}
