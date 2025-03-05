using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BudgetApp.Models;
using BudgetApp.Data;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Pages.Shared
{
    public class MainpageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MainpageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fetch categories dynamically for the dropdown
        public List<Category> Categories { get; set; } = new();

        public async Task OnGetAsync()
        {
            Categories = await _context.Categories.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string expenseName, decimal expenseAmount, string expenseCategory)
        {
            var category = await _context.Categories
                                         .FirstOrDefaultAsync(c => c.Name == expenseCategory);

            if (category == null)
            {
                return NotFound();
            }

            var expense = new Expense
            {
                Name = expenseName,
                Amount = expenseAmount,
                Date = DateTime.Now,
                CategoryId = category.Id
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}




