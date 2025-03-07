using Microsoft.AspNetCore.Identity;

namespace MyBlazorApp.Models
{
    // Custom user model extending IdentityUser.
    public class User : IdentityUser
    {
        // Navigation property: one User can have many Expenses.
        public List<Expense> Expenses { get; set; } = new();

    }
}
