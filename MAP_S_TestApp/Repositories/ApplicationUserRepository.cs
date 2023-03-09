using MAP_S_TestApp.Data;
using MAP_S_TestApp.Models.Domains;

namespace MAP_S_TestApp.Repositories;

public class ApplicationUserRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddUserAsync(ApplicationUser applicationUser)
    {
        _context.ApplicationUsers.Add(applicationUser);
        await _context.SaveChangesAsync();
    }

    public bool EmailAlreadyExists(string email)
    {
        return _context.ApplicationUsers.Any(x => x.Email == email);
    }

    public async Task<ApplicationUser> GetUserByEmailAsync(string email)
    {
        return await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<ApplicationUser> GetUserByIdAndTokenAsync(int applicationUserId, string verificationToken)
    {
        return await _context.ApplicationUsers.Where(x => x.Id == applicationUserId && x.VerificationToken == verificationToken).FirstOrDefaultAsync();
    }

    public async Task SetIsActivatedStatusTrueAsync(int applicationUserId)
    {
        var applicationUserToUpdate = _context.ApplicationUsers.Single(x => x.Id == applicationUserId);
        applicationUserToUpdate.IsActivated = true;
        _context.ApplicationUsers.Update(applicationUserToUpdate);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _context.ApplicationUsers.ToListAsync();
    }
}
