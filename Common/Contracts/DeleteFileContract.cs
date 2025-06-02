namespace Common.Contracts
{
    public record class DeleteFileContract
    {
        public string FileKey { get; set; } = string.Empty;
    }
}
