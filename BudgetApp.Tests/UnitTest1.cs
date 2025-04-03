using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BudgetApp.Tests{
    // Simple test models
    public class Expense
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;  // Initialize to avoid nullability warnings
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public Category? Category { get; set; }  // Make nullable to avoid warnings
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class UserBudget
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    // Test database context
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        
        public DbSet<Expense> Expenses { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<UserBudget> UserBudgets { get; set; } = null!;
    }

    // Basic test class - simplified for clarity
    public class BasicBudgetTests
    {
        private readonly TestDbContext _context;
        private readonly string _userId = "ture-c";
        
        public BasicBudgetTests()
        {
            // Create options for in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new TestDbContext(options);
            
            // Add test categories
            _context.Categories.Add(new Category { Id = 1, Name = "Food" });
            _context.Categories.Add(new Category { Id = 2, Name = "Entertainment" });
            _context.SaveChanges();
        }
        
        [Fact]
        public void Test_SetBudget_ShouldUpdateBudgetValue()
        {
            // Arrange
            decimal budgetAmount = 5000;
            
            // Act
            var userBudget = new UserBudget { UserId = _userId, Amount = budgetAmount };
            _context.UserBudgets.Add(userBudget);
            _context.SaveChanges();
            
            // Assert
            var savedBudget = _context.UserBudgets.FirstOrDefault(b => b.UserId == _userId);
            Assert.NotNull(savedBudget);
            Assert.Equal(budgetAmount, savedBudget.Amount);
        }
        
        [Fact]
        public void Test_AddExpense_ShouldCreateExpenseAndUpdateTotal()
        {
            // Arrange - Create budget first
            _context.UserBudgets.Add(new UserBudget { UserId = _userId, Amount = 5000 });
            _context.SaveChanges();
            
            // Act - Add expense
            var expense = new Expense
            {
                Name = "Groceries",
                Amount = 500,
                CategoryId = 1, // Food
                UserId = _userId,
                Date = DateTime.Now
            };
            
            _context.Expenses.Add(expense);
            _context.SaveChanges();
            
            // Assert
            var savedExpense = _context.Expenses.FirstOrDefault(e => e.UserId == _userId);
            Assert.NotNull(savedExpense);
            Assert.Equal("Groceries", savedExpense.Name);
            Assert.Equal(500, savedExpense.Amount);
            
            // Calculate total expenses
            var totalExpenses = _context.Expenses.Where(e => e.UserId == _userId).Sum(e => e.Amount);
            Assert.Equal(500, totalExpenses);
            
            // Calculate balance
            var budget = _context.UserBudgets.First(b => b.UserId == _userId).Amount;
            var balance = budget - totalExpenses;
            Assert.Equal(4500, balance);

           

        }
        [Fact]
        public void Test_EditExpense_ShouldUpdateExpenseDetails()
    {
    // Arrange - Add an initial expense
    _context.UserBudgets.Add(new UserBudget { UserId = _userId, Amount = 5000 });
    var expense = new Expense
    {
        Name = "Groceries",
        Amount = 500,
        CategoryId = 1, // Food
        UserId = _userId,
        Date = DateTime.Now
    };
    _context.Expenses.Add(expense);
    _context.SaveChanges();
    
    // Act - Update the expense
    var expenseToUpdate = _context.Expenses.First(e => e.UserId == _userId);
    expenseToUpdate.Name = "Dining Out";
    expenseToUpdate.Amount = 750;
    expenseToUpdate.CategoryId = 2; // Entertainment
    _context.SaveChanges();
    
    // Assert
    var updatedExpense = _context.Expenses.First(e => e.UserId == _userId);
    Assert.Equal("Dining Out", updatedExpense.Name);
    Assert.Equal(750, updatedExpense.Amount);
    Assert.Equal(2, updatedExpense.CategoryId);
    
    // Check total expenses
    var totalExpenses = _context.Expenses.Where(e => e.UserId == _userId).Sum(e => e.Amount);
    Assert.Equal(750, totalExpenses);
    }
    [Fact]
public void Test_DeleteExpense_ShouldRemoveExpenseAndUpdateTotal()
{
    // Arrange - Add an expense to delete
    _context.UserBudgets.Add(new UserBudget { UserId = _userId, Amount = 5000 });
    var expense = new Expense
    {
        Name = "Groceries",
        Amount = 500,
        CategoryId = 1, // Food
        UserId = _userId,
        Date = DateTime.Now
    };
    _context.Expenses.Add(expense);
    _context.SaveChanges();
    
    // Act - Delete the expense
    var expenseToDelete = _context.Expenses.First(e => e.UserId == _userId);
    _context.Expenses.Remove(expenseToDelete);
    _context.SaveChanges();
    
    // Assert
    var deletedExpense = _context.Expenses.FirstOrDefault(e => e.UserId == _userId);
    Assert.Null(deletedExpense);
    
    // Check total expenses
    var totalExpenses = _context.Expenses.Where(e => e.UserId == _userId).Sum(e => e.Amount);
    Assert.Equal(0, totalExpenses);
}

    }
}