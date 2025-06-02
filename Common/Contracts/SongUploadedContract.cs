namespace Common.Contracts
{
    public record SongUploadedContract
    {
        public string AuthorPseudonym { get; set; } = null!;
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Title { get; set; } = null!;
        public int GenreId { get; set; }
        public string SongFileKey { get; set; } = null!;
        public string? LogoFileKey { get; set; }
        public string? VideoClipFileKey { get; set; }
    }
}
