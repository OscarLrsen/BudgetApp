using Microsoft.AspNetCore.Identity;

namespace BudgetApp.Models
{
    public class User : IdentityUser
    {
        public int TransactionId { get; set; }
        public List<Transaction> Transactions { get; set; } = new(); // Navigation property, gör det lättare att hämta data

    }
}
