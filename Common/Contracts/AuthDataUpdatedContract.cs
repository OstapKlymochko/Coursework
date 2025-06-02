namespace Common.Contracts
{
    public record AuthDataUpdatedContract
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool EmailUpdated { get; set; } = false;
    }
}
