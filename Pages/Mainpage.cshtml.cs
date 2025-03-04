using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BudgetApp.Models;
using BudgetApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Options;


namespace BudgetApp.Pages.Shared
{
    public class MainpageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MainpageModel(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> OnPostAsync(string expenseTitle, decimal expenseAmount, string expenseCategory)
        {
            
            var category = await _context.Categories
                                         .FirstOrDefaultAsync(c => c.Name == expenseCategory);

            if (category == null)
            {
                
                return NotFound();
            }

            var expense = new Expense
            {
        //         <select id="expenseCategory" name="ExpenseCategory" required>
        //    <option value="1">Food</option>
        //    <option value="2">Transport</option>
        //    <option value="2">Utilities</option>
        //    <option value="4">Entertainment</option>
        //    <option value="5">Other</option>
        //    <option value="6">Amsterdam</option>
        //</select>


                //Title = expenseTitle,
                Amount = expenseAmount,
                Date = DateTime.Now,
                CategoryId = expenseCategory.ToString().Where(c =>
                {
                    return c.Id == expenseCategory;
                }).Id
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            //Logic för matte osv här
            return RedirectToPage();
        }
    }
}



