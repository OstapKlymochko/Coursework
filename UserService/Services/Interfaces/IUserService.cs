using Common.Result;
using UserService.Models;

namespace UserService.Services.Interfaces
{
	public interface IUserService
	{
		public Task CreateUser(int id);
		public Task<ServiceResult<UserModel>> UpdateUser(UserModel updateData);
		public Task<UserModel?> GetByIdAsync(int id);
	}
}
