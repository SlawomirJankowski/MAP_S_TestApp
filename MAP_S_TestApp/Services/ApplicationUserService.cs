using MAP_S_TestApp.Data;
using MAP_S_TestApp.Helpers;
using MAP_S_TestApp.Models.Domains;
using MAP_S_TestApp.Models.ViewModels;
using MAP_S_TestApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MAP_S_TestApp.Services;

public class ApplicationUserService : ControllerBase
{

    private readonly ApplicationUserRepository _applicationUserRepository;
    public ApplicationUserService(ApplicationDbContext context)
    {
        _applicationUserRepository = new ApplicationUserRepository(context);
    }

    public async Task AddUserAsync(RegisterUserViewModel registerUserModel, HttpContext httpContext)
    {
        PasswordHelper.CreatePasswordHash(
                registerUserModel.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var applicationUser = new ApplicationUser
        {
            FirstName = registerUserModel.FirstName,
            LastName = registerUserModel.LastName,
            Email = registerUserModel.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            CreatedAt = DateTime.UtcNow,
            VerificationToken = PasswordHelper.CreateVerificationToken()
        };

        await _applicationUserRepository.AddUserAsync(applicationUser);

        var link = GenerateConfirmationLink(applicationUser, httpContext);

        await SendConfirmationEmailAsync(link, applicationUser.FirstName, applicationUser.LastName, applicationUser.Email);
    }

    public bool EmailAlreadyExists(string email)
    {
        return _applicationUserRepository.EmailAlreadyExists(email);
    }

    public async Task<ApplicationUser> GetUserByEmailAsync(string email)
    {
        return await _applicationUserRepository.GetUserByEmailAsync(email);
    }

    public async Task<ApplicationUser> GetUserByIdAndTokenAsync(int applicationUserId, string verificationToken)
    {
        return await _applicationUserRepository.GetUserByIdAndTokenAsync(applicationUserId, verificationToken);
    }

    public async Task SetIsActivatedStatusTrueAsync(int applicationUserId)
    {
        await _applicationUserRepository.SetIsActivatedStatusTrueAsync(applicationUserId);
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _applicationUserRepository.GetAllUsersAsync();
    }

    public async Task<ApplicationUser> GetUserByIdAsync(int applicationUserId)
    {
        return await _applicationUserRepository.GetUserByIdAsync(applicationUserId);
    }

    public async Task UpdateApplicationUserAsync(ApplicationUser applicationUserModel)
    {
        var updatedApplicationUser = new ApplicationUser
        {
            Id = applicationUserModel.Id,
            FirstName = applicationUserModel.FirstName,
            LastName = applicationUserModel.LastName,
            Email = applicationUserModel.Email,
        };

        await _applicationUserRepository.UpdateApplicationUserAsync(updatedApplicationUser);
    }

    private static async Task SendConfirmationEmailAsync(string link, string firstName, string lastName, string email)
    {
        var emailSender = new EmailHelper();
        await emailSender.SendActivationLink(link, firstName, lastName, email);
    }

    private static string GenerateConfirmationLink(ApplicationUser applicationUser, HttpContext httpContext)
    {
        var link = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/Account/AccountConfirmation?applicationUserId={applicationUser.Id}&verificationToken={applicationUser.VerificationToken}";
        return link;
    }

    public string GetLoggedUserName(ClaimsPrincipal user)
    {
        var claims = user.Claims.ToList();
        return $"{claims[1].Value} {claims[2].Value}";
    }
}
