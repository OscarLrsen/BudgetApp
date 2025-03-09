using Blazor_Demo.Components; // adjust if needed
using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Data;
using MyBlazorApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register EF Core with your connection string.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Identity for your custom User model (for data structure only).
builder.Services.AddIdentityCore<User>(options => { /* options if needed */ })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorizationCore();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

// (We’re not calling UseAuthentication/UseAuthorization since we use a static test user)

// Minimal antiforgery middleware: get and store tokens so endpoints with antiforgery metadata are satisfied.
app.Use(async (context, next) =>
{
    var antiforgery = context.RequestServices.GetService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    if (antiforgery != null)
    {
        // This generates tokens and stores them in cookies.
        antiforgery.GetAndStoreTokens(context);
    }
    await next();
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
