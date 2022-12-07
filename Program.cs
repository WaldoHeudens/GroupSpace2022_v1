using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GroupSpace2022.Data;
using Microsoft.AspNetCore.Identity;
using GroupSpace2022.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Manueel toegevoegd om te werken met Identity Framework
var connectionString = builder.Configuration.GetConnectionString("GroupSpace2022Context_SQLServer");
// var connectionString = builder.Configuration.GetConnectionString("GroupSpace2022Context-LocalDB");

builder.Services.AddDbContext<GroupSpace2022Context>(options =>
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string 'GroupSpace2022Context' not found.")));

// Manueel toegevoegd om te werken met Identity Framework
builder.Services.AddDbContext<global::GroupSpace2022.Data.GroupSpace2022Context>((global::Microsoft.EntityFrameworkCore.DbContextOptionsBuilder options) =>
    options.UseSqlServer(connectionString));


builder.Services.AddDefaultIdentity<GroupSpace2022User>(options => options.SignIn.RequireConfirmedAccount = false)
// Manueel toegevoegd  om te werken met Identity Framework    
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<GroupSpace2022Context>();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<GroupSpace2022User>>();
    SeedDatacontext.Initialize(services, userManager);
}

// Manueel toegevoegd om te werken met Identity Framework
app.MapRazorPages();

app.Run();
