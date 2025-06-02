namespace Common.Contracts
{
	public record UsernameUpdatedContract
	{
		public int Id { get; set; }
		public string UserName { get; set; } = null!;
		public bool IsAuthor { get; set; }
	}
}
