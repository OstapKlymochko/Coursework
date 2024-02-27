using AuthService.Helpers;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Common.CommonTypes;
using Common.CommonTypes.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Common.Contracts;
using Common.Errors;
using Common.Result;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<IdentityUser<int>> _userManager;
		private readonly RoleManager<IdentityRole<int>> _roleManager;
		private readonly IJwtService _jwtService;
		private readonly IEmailService _emailService;
		private readonly IPublishEndpoint _publishEndpoint;
		private readonly IValidator<LoginUserModel> _loginUserValidator;
		private readonly IValidator<RegisterUserModel> _registerUserValidator;
		private readonly IValidator<UserDataModel> _updateUserValidator;

		private readonly IValidator<ConfirmEmailModel> _confirmEmailValidator;
		//todo remove to a separate service
		private readonly IRequestClient<GetUserData> _client;
		//private readonly Serilog.ILogger _logger;

		public AuthService(
			UserManager<IdentityUser<int>> userManager,
			IJwtService jwtService,
			IPublishEndpoint publishEndpoint,
			IValidator<LoginUserModel> loginUserValidator, IValidator<RegisterUserModel> registerUserValidator, IValidator<UserDataModel> updateUserValidator,
			IEmailService emailService, RoleManager<IdentityRole<int>> roleManager, IRequestClient<GetUserData> client, IValidator<ConfirmEmailModel> confirmEmailValidator /*, Serilog.ILogger logger*/)
		{
			_userManager = userManager;
			_jwtService = jwtService;
			_publishEndpoint = publishEndpoint;
			_loginUserValidator = loginUserValidator;
			_registerUserValidator = registerUserValidator;
			_emailService = emailService;
			_roleManager = roleManager;
			_client = client;
			_confirmEmailValidator = confirmEmailValidator;
			//_logger = logger;
			_updateUserValidator = updateUserValidator;
		}
		public async Task<ServiceResult<IBasicResponse>> RegisterUser(RegisterUserModel user)
		{
			var validationResult = await _registerUserValidator.ValidateAsync(user);
			if (!validationResult.IsValid)
				return new ModelError(string.Join(", ", validationResult.Errors));

			var u = await _userManager.FindByEmailAsync(user.Email);
			if (u != null)
				return new ModelError($"User with email {user.Email} already exists");

			var role = await _roleManager.FindByNameAsync(user.Role);
			if (role == null)
				return new ModelError($"Role {user.Role} does not exist");

			var identityUser = new IdentityUser<int>
			{
				Email = user.Email.Trim().ToLower(),
				UserName = user.Username.Trim()
			};
			var result = await _userManager.CreateAsync(identityUser, user.Password);
			if (!result.Succeeded) return new ModelError(result.MapErrors());

			await _userManager.AddToRoleAsync(identityUser, user.Role);
			await _publishEndpoint.Publish(new UserRegistered { UserId = identityUser.Id });
			await SendConfirmEmail(identityUser);

			return new BasicResponse() { Message = $"User {user.Username.Trim()} was created" };
		}

		public async Task<ServiceResult<TokenPairModel>> Login(LoginUserModel userCredentials)
		{
			var validationResult = await _loginUserValidator.ValidateAsync(userCredentials);
			if (!validationResult.IsValid)
				return new ModelError(string.Join(", ", validationResult.Errors));

			var user = await _userManager.FindByEmailAsync(userCredentials.Email);
			if (user == null)
				return new NotFoundError($"User with email {userCredentials.Email} does not exist");

			if (!user.EmailConfirmed)
			{
				await SendConfirmEmail(user);
				return new ModelError("You must confirm your email");
			}

			bool passwordsMatch = await _userManager.CheckPasswordAsync(user, userCredentials.Password);
			if (!passwordsMatch)
				return new ModelError("Wrong email or password");
			//_logger.Information($"User {user.Email} (Id = {user.Id}) signed in");
			return _jwtService.GenerateTokenPair(user);
		}

		public async Task<ServiceResult<BasicResponse>> RequestPasswordReset(UserIdentifierModel identifierModel)
		{
			if (string.IsNullOrWhiteSpace(identifierModel.Identifier))
				return new ModelError("Identifier must not be empty");

			var user = await FindUserByIdentifier(identifierModel.Identifier);
			if (user == null) return new NotFoundError("User does not exist");

			string token = _jwtService.SignResetPasswordToken(user);
			//todo replace with error controller handling, replace to config
			await _emailService.SendEmailAsync($"<h2>Click <a href=\"http://localhost:3000/reset-password?token={token}\" target=\"_blank\">here</a> to restore your password</h2>",
				"Reset password", user.Email!);

			return new BasicResponse($"An email was sent to your address. ");
		}

		public async Task<ServiceResult<BasicResponse>> ResetPassword(ResetPasswordModel resetPasswordModel)
		{
			if (string.IsNullOrWhiteSpace(resetPasswordModel.Password)) return new ModelError("Password must not be empty");

			var tokenData = _jwtService.ValidateResetPasswordToken(resetPasswordModel.ValidationToken);
			if (tokenData == null) return new ModelError("Invalid validation token. Please, try resetting your password again");

			int userId = (int)tokenData.UserId!;
			var user = await _userManager.FindByIdAsync(userId.ToString());
			if (user == null) return new NotFoundError("User does not exist");

			bool passwordsMatch = await _userManager.CheckPasswordAsync(user, resetPasswordModel.Password);
			if (passwordsMatch) return new ModelError("You can not set your old password");
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordModel.Password);
			if (!result.Succeeded) return new ModelError(result.MapErrors());

			return new BasicResponse("Password was successfully reset");
		}

		public async Task<ServiceResult<UserDataModel>> GetUserData(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return new NotFoundError("User not found");
			var r = await _client.GetResponse<GetUserDataResponse, NotFoundError>(new GetUserData() { UserId = int.Parse(id) });
			if (r.Is(out Response<NotFoundError>? notFoundResponse)) return notFoundResponse.Message;
			if (r.Is(out Response<GetUserDataResponse>? userDataResponse)) return new UserDataModel(user, userDataResponse.Message);
			return new InternalError("Something went wrong");
		}

		public async Task<ServiceResult<BasicResponse>> UpdateUserData(UserDataModel updateUserData)
		{
			var validationResult = await _updateUserValidator.ValidateAsync(updateUserData);
			if (!validationResult.IsValid) return new ModelError(string.Join(", ", validationResult.Errors));

			var user = await _userManager.FindByIdAsync(updateUserData.Id.ToString());
			if (user == null) return new NotFoundError("User not found");
			if (user.NormalizedEmail != updateUserData.Email.Trim().ToUpper())
			{
				bool isEmailUnique = await _userManager.FindByEmailAsync(updateUserData.Email) == null;
				if (!isEmailUnique) return new ModelError($"Email {user.Email} is already taken");
			}
			user.Email = updateUserData.Email.Trim().ToLower();
			user.UserName = updateUserData.UserName;
			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded) return new ModelError(result.MapErrors());
			await _publishEndpoint.Publish(new UpdateUserData() { FirstName = updateUserData.FirstName, LastName = updateUserData.LastName, Id = updateUserData.Id });
			return new BasicResponse("Updated successfully");
		}

		public async Task<ServiceResult<TokenPairModel>> Refresh(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) return new ModelError("User does not exist");

			return _jwtService.GenerateTokenPair(user);
		}
		//Todo move to common
		public int ExtractIdFromToken(HttpContext ctx)
		{
			return int.Parse(ctx.User.Claims.First(c => c.Type == "userId").Value);
		}

		private async Task<IdentityUser<int>?> FindUserByIdentifier(string identifier)
		{
			var normalized = identifier.Trim().ToUpper();
			return await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalized || u.NormalizedUserName == normalized || u.PhoneNumber == normalized);
		}

		public async Task<ServiceResult<BasicResponse>> ConfirmEmail(ConfirmEmailModel confirmModel)
		{
			var validationResult = await _confirmEmailValidator.ValidateAsync(confirmModel);
			if (!validationResult.IsValid)
				return new ModelError(string.Join(", ", validationResult.Errors));
			var user = await _userManager.FindByEmailAsync(confirmModel.Email);
			if (user == null)
				return new NotFoundError("User does not exist");
			var result = await _userManager.ConfirmEmailAsync(user, confirmModel.Token);
			if (!result.Succeeded)
				return new ModelError(result.MapErrors());
			return new BasicResponse("Email was successfully confirmed");
		}

		private async Task SendConfirmEmail(IdentityUser<int> user)
		{
			string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			await _emailService.SendEmailAsync($"<h2>Click <a href=\"http://localhost:3000/confirm-email?token={token}\" target=\"_blank\">here</a> to confirm your email</h2>",
				"Email confirmation", user.Email!);
		}

	}
}

