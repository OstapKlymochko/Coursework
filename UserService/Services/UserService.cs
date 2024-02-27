using Common.CommonTypes.Interfaces;
using Common.Errors;
using Common.Result;
using FluentValidation;
using UserService.Models;
using UserService.Repositories.Interfaces;
using UserService.Services.Interfaces;

namespace UserService.Services
{
	public class UserService : IUserService
	{
		private readonly IGenericRepository<UserModel> _userRepository;
		private readonly IValidator<UserModel> _userModelValidator;
		public UserService(IGenericRepository<UserModel> userRepository, IValidator<UserModel> userModelValidator)
		{
			_userRepository = userRepository;
			_userModelValidator = userModelValidator;
		}

		public async Task CreateUser(int id)
		{
			var user = new UserModel() { Id = id };
			await _userRepository.CreateAsync(user);
		}

		public async Task<ServiceResult<UserModel>> UpdateUser(UserModel updateData)
		{
			var result = await _userModelValidator.ValidateAsync(updateData);
			if (!result.IsValid) return new ModelError(string.Join(' ', result.Errors));
			var user = await _userRepository.GetByIdAsync(updateData.Id);
			if (user == null) return new NotFoundError($"User with id {updateData.Id} does not exist");
			await _userRepository.UpdateAsync(updateData);
			return updateData;
		}

		public async Task<UserModel?> GetByIdAsync(int id)
		{
			return await _userRepository.GetByIdAsync(id);
		}

	}
}
