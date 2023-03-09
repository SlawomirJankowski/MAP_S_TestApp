using MAP_S_TestApp.Models.Domains;

namespace MAP_S_TestApp.Models.ViewModels;

public class AllApplicationUsersViewModel
{
    public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
}
