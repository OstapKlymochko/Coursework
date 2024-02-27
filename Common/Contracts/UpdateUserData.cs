namespace Common.Contracts
{
	public record UpdateUserData
	{
        public int Id { get; set; }
        public string? FirstName { get; set; }
		public string? LastName { get; set; }
	}
}
