using FilesService.Attributes;
using System.ComponentModel.DataAnnotations;

namespace FilesService.Models
{
    public class UploadSongModel
    {
        [Required(ErrorMessage = "Title must be specified")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Genre must be specified")]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Song file must be attached")]
        [MaxFileSize(1024 * 1024 * 5, ErrorMessage = "Song file is too big")]
        [AllowedExtensions(new string[] { ".mp3", ".mpeg" })]
        public IFormFile Song { get; set; } = null!;

        [MaxFileSize(1024 * 1024 * 1, ErrorMessage = "Logo file is too big")]
        [AllowedExtensions(new string[] { ".jpeg", ".png" })]
        public IFormFile? Logo { get; set; }

        [MaxFileSize(1024 * 1024 * 5, ErrorMessage = "Videoclip file is too big")]
        [AllowedExtensions(new string[] { ".mp4" })]
        public IFormFile? VideoClip { get; set; }

        [Required(ErrorMessage = "Author's pseudonym must be specified")]
        public string Pseudonym { get; set; } = null!;
    }
}
