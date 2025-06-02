namespace Common.Contracts
{
    public record AvatarUploadedContract
    {
        public int UserId { get; set; }
        public string FileKey { get; set; } = null!;
    }
}
