using Blazor_Demo.Components;
using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Data;
using MyBlazorApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register the EF Core DbContext using the connection string from appsettings.json.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity for the custom User model. 
// (For .NET 8, you may need to use a preview version of these packages.)
builder.Services.AddIdentityCore<User>(options => { /* configure options as needed */ })
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


app.MapRazorComponents<App>()  // 'App' should be your root component.
    .AddInteractiveServerRenderMode();

app.Run();
