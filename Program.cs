using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GroupSpace2022.Data;
using Microsoft.AspNetCore.Identity;
using GroupSpace2022.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Manueel toegevoegd om te werken met Identity Framework
var connectionString = builder.Configuration.GetConnectionString("GroupSpace2022Context_SQLServer");
// connectionString = builder.Configuration.GetConnectionString("GroupSpace2022Context-LocalDB");

builder.Services.AddDbContext<GroupSpace2022Context>(options =>
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string 'GroupSpace2022Context' not found.")));

// Manueel toegevoegd om te werken met Identity Framework
builder.Services.AddDbContext<global::GroupSpace2022.Data.IdentityDbContext>((global::Microsoft.EntityFrameworkCore.DbContextOptionsBuilder options) =>
    options.UseSqlServer(connectionString));


builder.Services.AddDefaultIdentity<GroupSpace2022User>(options => options.SignIn.RequireConfirmedAccount = true)
// Manueel toegevoegd  om te werken met Identity Framework    
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>();


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedDatacontext.Initialize(services);
}

// Manueel toegevoegd om te werken met Identity Framework
app.MapRazorPages();

app.Run();
