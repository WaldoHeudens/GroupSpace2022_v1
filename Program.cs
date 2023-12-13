using Microsoft.EntityFrameworkCore;
using GroupSpace2022.Data;
using Microsoft.AspNetCore.Identity;
using GroupSpace2022.Areas.Identity.Data;
using GroupSpace2022.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using NETCore.MailKit.Infrastructure.Internal;
using GroupSpace2022.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

// Manueel toegevoegd om te werken met verschillende databases in Identity Framework
var connectionString = builder.Configuration.GetConnectionString("GroupSpace2022Context_SQLServer");
//var connectionString = builder.Configuration.GetConnectionString("GroupSpace2022Context-LocalDB");

builder.Services.AddDbContext<GroupSpace2022Context>(options =>
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string 'GroupSpace2022Context' not found.")));

// Manueel toegevoegd om te werken met Identity Framework
builder.Services.AddDbContext<global::GroupSpace2022.Data.GroupSpace2022Context>((global::Microsoft.EntityFrameworkCore.DbContextOptionsBuilder options) =>
    options.UseSqlServer(connectionString));


builder.Services.AddDefaultIdentity<GroupSpace2022User>(options => options.SignIn.RequireConfirmedAccount = true)
// Manueel toegevoegd  om te werken met Identity Framework    
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<GroupSpace2022Context>();

builder.Services.AddLocalization(option => option.ResourcesPath = "Meertaligheid");
builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IEmailSender, MailKitEmailSender>();

// De volgende configuratie gebruiken we niet meer, maar zit er nog steeds als voorbeeld in.
// De eigenlijke implementatie gebeurd d.m.v. de parameters:  Zie Globals!
builder.Services.Configure<MailKitOptions>(options =>
{
    options.Server = builder.Configuration["ExternalProviders:MailKit:SMTP:Address"];
    options.Port = Convert.ToInt32(builder.Configuration["ExternalProviders:MailKit:SMTP:Port"]);
    options.Account = builder.Configuration["ExternalProviders:MailKit:SMTP:Account"];
    options.Password = builder.Configuration["ExternalProviders:MailKit:SMTP:Password"];
    options.SenderEmail = builder.Configuration["ExternalProviders:MailKit:SMTP:SenderEmail"];
    options.SenderName = builder.Configuration["ExternalProviders:MailKit:SMTP:SenderName"];

    // Set it to TRUE to enable ssl or tls, FALSE otherwise
    options.Security = true;  // true zet ssl or tls aan
});

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

// Zorg ervoor dat TempData beschouwd wordt als essentiële cookie, en dus altijd bestaat
builder.Services.Configure<CookieTempDataProviderOptions>(options => {
    options.Cookie.IsEssential = true;
});

// Voorbereiding voor het gebruik van RestFull API
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "GroupSpace2022", Version = "v1" });
});

var app = builder.Build();
Globals.App = app;


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<GroupSpace2022User>>();
    await SeedDatacontext.Initialize(services, userManager);
}


// zorg dat het systeem beschikt over een lijst van gebruikte cultures
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("nl-BE")
    .AddSupportedCultures(Language.SupportedCultures)
    .AddSupportedUICultures(Language.SupportedCultures);

app.UseRequestLocalization(localizationOptions);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Manueel toegevoegd om te werken met Identity Framework dat niet in MVC geschreven is
app.MapRazorPages();

// Voer de "Globals" middleware uit
app.UseMiddleware<Globals>();

// Voeg toe voor het voorzien van 
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();
