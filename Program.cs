using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GroupSpace2022.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GroupSpace2022Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GroupSpace2022Context") ?? throw new InvalidOperationException("Connection string 'GroupSpace2022Context' not found.")));

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedDatacontext.Initialize(services);
}

app.Run();
