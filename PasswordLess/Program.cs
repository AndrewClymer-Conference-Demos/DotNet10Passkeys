using PasswordLess;
using PasswordLess.Pages.Account;
using PasswordLess.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers(); // Add support for API controllers
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
       
    });

builder.Services.Configure<IdentityPasskeyOptions>(options =>
{
    options.AuthenticatorTimeout = TimeSpan.FromMinutes(3);
    options.ChallengeSize = 64;
});


builder.Services
    .AddIdentity<IdentityUser,IdentityRole>(options =>
        {
            options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;  
        })
    .AddEntityFrameworkStores<ApplicationDbContext>()
   .AddDefaultTokenProviders(); // Configure Identity services

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Set your custom login path
    options.AccessDeniedPath = "/Account/Login"; // Optional: Set an access denied path
});


builder.Services.AddSingleton<IVerifyAccountOwnership,CookieVerifyAccountOwnership>();

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
app.UseSession();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();
app.MapControllers(); // Map API controllers


app.Run();