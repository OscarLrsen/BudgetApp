using BudgetApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Apply migrations to the database
    //dbContext.Database.Migrate();

    // Seed the database with initial data
    SampleData.SeedData(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/api/budget", async (HttpContext context, [FromServices] ApplicationDbContext dbContext, [FromServices] SignInManager<IdentityUser> signInManager) =>
{
    var userId = signInManager.UserManager.GetUserId(context.User);
    if (string.IsNullOrEmpty(userId))
    {
        return Results.Unauthorized();
    }

    var userBudget = await dbContext.UserBudgets.FirstOrDefaultAsync(b => b.UserId == userId);
    var expenses = await dbContext.Expenses.Where(e => e.UserId == userId).ToListAsync();

    var budgetAmount = userBudget?.Amount ?? 0;
    var totalExpenses = expenses.Sum(e => e.Amount);

    return Results.Json(new { budgetAmount, totalExpenses });
});


app.Run();
