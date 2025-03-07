namespace MyBlazorApp.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        // Foreign key for the associated user.
        public string UserId { get; set; }
        public User User { get; set; }

        // Foreign key for the associated category.
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
