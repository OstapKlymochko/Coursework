namespace MusicService.Models
{
    public class UploadSongModel
    {
        public string Title { get; set; } = null!;
        public int GenreId { get; set; }
        public IFormFile Song { get; set; } = null!;
        public IFormFile? Logo { get; set; }
        public IFormFile? VideoClip { get; set; }
        public string Pseudonym { get; set; } = null!;
    }
}
