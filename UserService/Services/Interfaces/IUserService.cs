using Common.CommonTypes;
using Common.Result;
using UserService.Models;

namespace UserService.Services.Interfaces
{
    public interface IUserService
    {
        public Task CreateUser(int id, string email, string userName);
        public Task<ServiceResult<BasicResponse>> UpdateUser(UpdateUserModel updateData, bool isAuthor = false);
        public Task SetAvatarAsync(string fileKey, int userId);
        public Task<ServiceResult<UserDto>> GetByIdAsync(int id);
    }
}
