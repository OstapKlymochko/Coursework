using AuthService.Helpers;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Common.CommonTypes;
using Common.CommonTypes.Interfaces;
using Microsoft.AspNetCore.Identity;
using Common.Contracts;
using Common.Errors;
using Common.Result;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Common.Services.Interfaces;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser<int>> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IMessageQueueService _messageQueueService;
        private readonly IValidator<RegisterUserModel> _registerUserValidator;
        private readonly string _clientUrl;

        public AuthService(
            UserManager<IdentityUser<int>> userManager, RoleManager<IdentityRole<int>> roleManager,
            IJwtService jwtService, IValidator<RegisterUserModel> registerUserValidator,
            IMessageQueueService messageQueueService, IEmailService emailService, IConfiguration cfg)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _emailService = emailService;
            _messageQueueService = messageQueueService;
            _registerUserValidator = registerUserValidator;
            _clientUrl = cfg.GetValue<string>("ClientUrl")!;
        }
        public async Task<ServiceResult<IBasicResponse>> RegisterUserAsync(RegisterUserModel user)
        {
            var validationResult = await _registerUserValidator.ValidateAsync(user);
            if (!validationResult.IsValid) return new ModelError(string.Join(", ", validationResult.Errors));

            var u = await _userManager.FindByEmailAsync(user.Email);
            if (u != null) return new ModelError($"User with email {user.Email} already exists");

            var role = await _roleManager.FindByNameAsync(user.Role);
            if (role == null) return new ModelError($"Role {user.Role} does not exist");

            var identityUser = new IdentityUser<int>
            {
                Email = user.Email.Trim().ToLower(),
                UserName = user.UserName
            };

            var result = await _userManager.CreateAsync(identityUser, user.Password);
            if (!result.Succeeded) return new ModelError(result.MapErrors());

            await _userManager.AddToRoleAsync(identityUser, user.Role);
            await _messageQueueService.PublishMessageAsync(new UserRegistered { UserId = identityUser.Id, UserName = user.UserName!, Email = user.Email.Trim().ToLower() });
            await SendConfirmEmailAsync(identityUser);

            return new BasicResponse() { Message = $"You've successfully signed up!" };
        }

        public async Task<ServiceResult<TokenPairModel>> LoginAsync(LoginUserModel userCredentials)
        {
            if (string.IsNullOrEmpty(userCredentials.Email) || string.IsNullOrEmpty(userCredentials.Password))
                return new ModelError("Email and password must be specified");

            var user = await _userManager.FindByEmailAsync(userCredentials.Email);
            if (user == null) return new NotFoundError($"User with email {userCredentials.Email} does not exist");

            if (!user.EmailConfirmed)
            {
                await SendConfirmEmailAsync(user);
                return new ModelError("You must confirm your email");
            }

            bool passwordsMatch = await _userManager.CheckPasswordAsync(user, userCredentials.Password);
            if (!passwordsMatch) return new ModelError("Wrong email or password");

            var roles = await _userManager.GetRolesAsync(user);
            return _jwtService.GenerateTokenPair(user, roles);
        }

        public async Task<ServiceResult<BasicResponse>> RequestPasswordResetAsync(UserIdentifierModel identifierModel)
        {
            if (string.IsNullOrWhiteSpace(identifierModel.Identifier))
                return new ModelError("Identifier must not be empty");

            var user = await FindUserByIdentifier(identifierModel.Identifier);
            if (user == null) return new NotFoundError("User does not exist");

            string token = _jwtService.SignResetPasswordToken(user);
            //todo replace with error controller handling, replace to config
            await _emailService.SendEmailAsync($"<h2>Click <a href=\"{_clientUrl}/reset-password?token={token}\" target=\"_blank\">here</a> to restore your password</h2>",
                "Reset password", user.Email!);

            return new BasicResponse($"An email was sent to your address.");
        }

        public async Task<ServiceResult<BasicResponse>> ResetPasswordAsync(ResetPasswordModel resetPasswordModel)
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

        public async Task<ServiceResult<TokenPairModel>> RefreshAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new ModelError("User does not exist");
            if (!user.EmailConfirmed)
            {
                await SendConfirmEmailAsync(user);
                return new ModelError("You must confirm your email");
            }
            var roles = await _userManager.GetRolesAsync(user);
            return _jwtService.GenerateTokenPair(user, roles);
        }

        private Task<IdentityUser<int>?> FindUserByIdentifier(string identifier)
        {
            var normalized = identifier.Trim().ToUpper();
            return _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalized || u.NormalizedUserName == normalized || u.PhoneNumber == normalized);
        }

        public async Task<ServiceResult<BasicResponse>> ConfirmEmailAsync(ConfirmEmailModel confirmModel)
        {
            if (string.IsNullOrEmpty(confirmModel.Email) || string.IsNullOrEmpty(confirmModel.Token))
                return new ModelError("Email and token must be specified");

            var user = await _userManager.FindByEmailAsync(confirmModel.Email);
            if (user == null) return new NotFoundError("User does not exist");

            var result = await _userManager.ConfirmEmailAsync(user, confirmModel.Token);
            if (!result.Succeeded) return new ModelError(result.MapErrors());

            return new BasicResponse("Email was successfully confirmed");
        }

        public async Task SendConfirmEmailAsync(IdentityUser<int> user)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailService.SendEmailAsync($"<h2>Click <a href=\"{_clientUrl}/confirm-email?token={token}&email={user.NormalizedEmail}\" target=\"_blank\">here</a> to confirm your email</h2>",
                "Email confirmation", user.Email!);
        }

    }
}

