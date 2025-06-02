using Common.CommonTypes;
using Common.CommonTypes.Interfaces;
using Common.Contracts;
using Common.Errors;
using Common.Result;
using Common.Services.Interfaces;
using FluentValidation;
using UserService.Models;
using UserService.Services.Interfaces;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<UserModel> _userRepository;
        private readonly IFileLinkGeneratorService _fileLinkGeneratorService;
        private readonly IMessageQueueService _queueService;
        private readonly IValidator<UpdateUserModel> _userModelValidator;

        public UserService(IGenericRepository<UserModel> userRepository,
            IValidator<UpdateUserModel> userModelValidator,
            IFileLinkGeneratorService fileLinkGeneratorService,
            IMessageQueueService queueService)
        {
            _userRepository = userRepository;
            _userModelValidator = userModelValidator;
            _fileLinkGeneratorService = fileLinkGeneratorService;
            _queueService = queueService;
        }

        public async Task CreateUser(int id, string email, string userName)
        {
            var user = new UserModel() { Id = id, Email = email, UserName = userName };
            await _userRepository.CreateAsync(user);
        }

        public async Task<ServiceResult<BasicResponse>> UpdateUser(UpdateUserModel updateData, bool isAuthor = false)
        {
            var result = await _userModelValidator.ValidateAsync(updateData);
            if (!result.IsValid) return new ModelError(string.Join(' ', result.Errors));

            var user = await _userRepository.GetByIdAsync(updateData.Id);
            if (user == null) return new NotFoundError($"User with id {updateData.Id} does not exist");

            if (user.Email != updateData.Email || user.UserName != updateData.UserName)
            {
                var message = new AuthDataUpdatedContract() 
                {
                    Email = updateData.Email!,
                    UserName = updateData.UserName!, 
                    EmailUpdated = user.Email != updateData.Email,
                    Id = user.Id
                };
                var error = await _queueService.PublishMessageAndHandleErrorAsync<AuthDataUpdatedContract, ModelError>(message);
                if (error != null) return error;
            }

            user.UpdateUser(updateData);
            await _userRepository.UpdateAsync(user);
            return new BasicResponse("Updated succesfully");
        }

        public async Task SetAvatarAsync(string fileKey, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user!.AvatarFileKey != null)
            {
                var message = new DeleteFileContract() { FileKey = user.AvatarFileKey };
                await _queueService.PublishMessageAsync(message);
            }
            user!.AvatarFileKey = fileKey;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<ServiceResult<UserDto>> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return new NotFoundError("User does not exist");
            var userDto = new UserDto(user);
            if (user.AvatarFileKey != null) userDto.AvatarUrl = await _fileLinkGeneratorService.GetPreSignedUrl(user.AvatarFileKey);
            return userDto;
        }

    }
}
