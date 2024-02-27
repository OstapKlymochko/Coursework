namespace Common.Contracts
{
	public record UserNotFound
	{
		public int Id { get; set; }
		public string Message { get; set; } = null!;
	}
}
