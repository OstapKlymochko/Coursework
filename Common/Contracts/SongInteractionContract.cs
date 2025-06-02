namespace Common.Contracts
{
    public record SongInteractionContract
    {
        public int UserId { get; set; }
        public int SongId { get; set; }
        public string InteractionType { get; set; } = null!;
        public double? ListenTime { get; set; }
        public bool Toggle { get; set; } = false;
        public bool Delete { get; set; } = false;
    }
}
