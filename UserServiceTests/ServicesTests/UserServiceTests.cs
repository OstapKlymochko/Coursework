using Common.CommonTypes;
using Common.CommonTypes.Interfaces;
using Common.Contracts;
using Common.Errors;
using Common.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using UserService.Models;
using UserService.Services;
using UserService.Services.Interfaces;

namespace UserServiceTests.ServicesTests
{
    public class UserServiceTests
    {
        private readonly Mock<IGenericRepository<UserModel>> _userRepositoryMock;
        private readonly Mock<IFileLinkGeneratorService> _fileLinkGeneratorServiceMock;
        private readonly Mock<IMessageQueueService> _messageQueServiceMock;
        private readonly Mock<IValidator<UpdateUserModel>> _userModelValidatorMock;

        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _userModelValidatorMock = new Mock<IValidator<UpdateUserModel>>();
            _userRepositoryMock = new Mock<IGenericRepository<UserModel>>();
            _fileLinkGeneratorServiceMock = new Mock<IFileLinkGeneratorService>();
            _messageQueServiceMock = new Mock<IMessageQueueService>();

            _userService = new UserService.Services.UserService(_userRepositoryMock.Object, _userModelValidatorMock.Object,
                _fileLinkGeneratorServiceMock.Object, _messageQueServiceMock.Object);
        }

        [Fact]
        public async Task UserService_CreateUser()
        {
            var user = new UserModel()
            {
                Email = "asdqwe@gmail.com",
                UserName = "okl",
                Id = 1
            };

            _userRepositoryMock.Setup(u => u.CreateAsync(user, false)).Returns(Task.FromResult((object)null!)).Verifiable();

            await _userService.CreateUser(user.Id, user.Email, user.UserName);
        }

        [Fact]
        public async Task UserService_UpdateUser()
        {
            var user = new UpdateUserModel() { Id = 1, FirstName = "asdqwe" };
            var userModel = new UserModel() { Id = 1 };

            _userModelValidatorMock
                .Setup(u => u.ValidateAsync(GetAny<UpdateUserModel>(), GetAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1)).ReturnsAsync(userModel);
            _messageQueServiceMock
                .Setup(m => m.PublishMessageAndHandleErrorAsync<AuthDataUpdatedContract, ModelError>(GetAny<AuthDataUpdatedContract>()))
                .ReturnsAsync((ModelError)null!);

            _userRepositoryMock.Setup(u => u.UpdateAsync(GetAny<UserModel>(), false)).Returns(Task.FromResult((object)null!)).Verifiable();

            var result = await _userService.UpdateUser(user, true);

            Assert.True(result.IsSuccess);
            Assert.IsType<BasicResponse>(result.Value);
        }

        [Fact]
        public async Task UserService_SetAvatarAsync()
        {
            var user = new UserModel() { Id = 1, AvatarFileKey = "test" };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(1)).ReturnsAsync(user);
            _userRepositoryMock.Setup(u => u.UpdateAsync(GetAny<UserModel>(), false)).Returns(Task.FromResult((object)null!)).Verifiable();

            await _userService.SetAvatarAsync(GetAny<string>(), 1);
        }

        [Fact]
        public async Task UserService_GetByIdAsync()
        {
            var user = new UserModel() { Id = 1, AvatarFileKey = "test" };
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1)).ReturnsAsync(user);
            _fileLinkGeneratorServiceMock.Setup(f => f.GetPreSignedUrl(GetAny<string>())).ReturnsAsync("success");

            var result = await _userService.GetByIdAsync(1);

            Assert.True(result.IsSuccess);
            Assert.IsType<UserDto>(result.Value);
            Assert.Equal(1, result.Value.Id);
        }

        private T GetAny<T>() => It.IsAny<T>();
    }
}
