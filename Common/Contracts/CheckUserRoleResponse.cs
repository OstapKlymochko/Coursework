namespace Common.Contracts
{
	public record CheckUserRoleResponse
	{
		public bool IsAuthor { get; set; } = false;
	}
}
