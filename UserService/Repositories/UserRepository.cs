using Common.Services;
using UserService.Models;
using UserService.Repositories.Interfaces;
using UserService.Services.Interfaces;

namespace UserService.Repositories
{
	public class UserRepository: IUserRepository<UserModel>
	{
		private readonly IDataAccessService _accessService;

		public UserRepository(IDataAccessService accessService)
		{
			_accessService = accessService;
		}

		public async Task<IEnumerable<UserModel>> GetAllAsync()
		{
			return await _accessService.QueryDataAsync<UserModel, dynamic>("select * from users;", null);
		}

		public async Task<UserModel?> GetByIdAsync(int id)
		{
			return await _accessService.QuerySingleRecordAsync<UserModel, dynamic>("select * from users where id=@Id", new {Id = id});
		}

		public async Task AddAsync(int id)
		{
			await _accessService.ExecuteStatementAsync("insert into users (Id) values(@Id)", new { Id = id });
		}

		public async Task UpdateAsync(UserModel entity)
		{
			await _accessService.ExecuteStatementAsync(
				"update users set firstname=@FirstName, lastname=@LastName where id=@Id", entity);
		}

		public async Task DeleteAsync(int id)
		{
			await _accessService.ExecuteStatementAsync("delete from users where id=@Id", new {Id = id});
		}
	}
}
