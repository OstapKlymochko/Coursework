using Common.Contracts;

namespace MusicService.Models
{
	public class AuthorData
	{
		public AuthorData()
		{
		}

		public AuthorData(GetUserDataResponse user)
		{
			Id = user.Id;
			FirstName = user.FirstName;
			LastName = user.LastName;
		}

		public int Id { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
	}
}
