using DIgitalFrontDoor;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers(); // Add support for API controllers

builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
       
    });

builder.Services
    .AddIdentity<IdentityUser,IdentityRole>()
    .AddUserStore<InMemoryUserStore>()
    .AddRoleStore<InMemoryRoleStore>()
   .AddDefaultTokenProviders(); // Configure Identity services


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Set your custom login path
    options.AccessDeniedPath = "/Account/Login"; // Optional: Set an access denied path
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();
app.MapControllers(); // Map API controllers

app.Run();