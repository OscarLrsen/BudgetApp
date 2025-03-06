using BudgetApp.Data;
using BudgetApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Pages
{
    public class MainpageModel : PageModel
    {
        //Database context and sign-in manager
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;

        public MainpageModel(ApplicationDbContext context, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // Properties för att ladda data från db och visa i vyn 
        public List<Category> Categories { get; set; } = new();
        public List<Expense> Expenses { get; set; } = new();

        [BindProperty]
        public decimal Budget { get; set; }
        public decimal TotalExpenses { get; set; }

        public async Task OnGetAsync()
        {
            // Laddar kategorier för dropdown.
            Categories = await _context.Categories.ToListAsync();

            // Skaffa nuvarande user id
            var userId = _signInManager.UserManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Ladda expenses från databasen
            Expenses = await _context.Expenses
                                     .Where(e => e.UserId == userId)
                                     .Include(e => e.Category)
                                     .ToListAsync();

            // kalkulerar totalbelopp för expenses
            TotalExpenses = Expenses.Sum(e => e.Amount);

            // Ladda userbudget från databasen
            var userBudget = await _context.UserBudgets.FirstOrDefaultAsync(b => b.UserId == userId);
            if (userBudget != null)
            {
                Budget = userBudget.Amount;
            }
        }

        // Metod för att sätta budget
        public async Task<IActionResult> OnPostSetBudgetAsync(decimal budget)
        {
            var userId = _signInManager.UserManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }
            
            var userBudget = await _context.UserBudgets.FirstOrDefaultAsync(b => b.UserId == userId);
            if (userBudget == null)
            {
                userBudget = new UserBudget { UserId = userId, Amount = budget };
                _context.UserBudgets.Add(userBudget);
            }
            else
            {
                userBudget.Amount = budget;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
        // Metod för att lägga till en ny utgift
        public async Task<IActionResult> OnPostAddExpenseAsync(string expenseName, decimal expenseAmount, int expenseCategory)
        {
            var userId = _signInManager.UserManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Laddar kategorier för dropdown
            Categories = await _context.Categories.ToListAsync();

            // Validera att alla fält är ifyllda och att beloppet är större än 0
            if (string.IsNullOrEmpty(expenseName) || expenseAmount <= 0)
            {
                ModelState.AddModelError(string.Empty, "Alla fält måste vara ifyllda och beloppet måste vara större än 0.");
                Expenses = await _context.Expenses.Where(e => e.UserId == userId)
                                                  .Include(e => e.Category)
                                                  .ToListAsync();
                TotalExpenses = Expenses.Sum(e => e.Amount);
                return Page();
            }

            // Validera att kategorin finns
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == expenseCategory);
            if (category == null)
            {
                return NotFound();
            }

            // Skapa en ny utgift och spara i databasen
            var expense = new Expense
            {
                Name = expenseName,
                Amount = expenseAmount,
                Date = DateTime.Now,
                CategoryId = category.Id,
                UserId = userId
            };
            // Lägger till utgiften i databasen
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
