namespace MyBlazorApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property: one Category can have many Expenses.
        public List<Expense> Expenses { get; set; } = new();
    }
}
