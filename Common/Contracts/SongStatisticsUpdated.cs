namespace Common.Contracts
{
    public record SongStatisticsUpdated
    {
        public int SongId { get; set; }
        public int? Likes { get; set; }
        public int? Dislikes { get; set; }
        public int? Comments { get; set; }
        
    }
}
