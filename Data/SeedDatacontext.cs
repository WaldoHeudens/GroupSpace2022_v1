using Microsoft.EntityFrameworkCore;
using GroupSpace2022.Models;
using Microsoft.AspNetCore.Identity;
using GroupSpace2022.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace GroupSpace2022.Data
{
    public class SeedDatacontext
    {
        public static async Task<IActionResult> Initialize(System.IServiceProvider serviceProvider, UserManager<GroupSpace2022User> userManager)
        {
            using (var context = new GroupSpace2022Context(serviceProvider.GetRequiredService<DbContextOptions<GroupSpace2022Context>>()))
            {
                context.Database.Migrate();
                context.Database.EnsureCreated();   // Zorg dat de databank bestaat

                if (!context.Roles.Any())
                {

                    GroupSpace2022User dummy = new GroupSpace2022User
                    {
                        Email = "?@?.?",
                        EmailConfirmed = true,
                        LockoutEnabled = true,
                        UserName = "dummy",
                        FirstName = "?",
                        LastName = "?"
                        };
                    GroupSpace2022User administrator = new GroupSpace2022User
                    {
                        Email = "admin@Groupspace2022.be",
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        UserName = "Administrator",
                        FirstName = "Administrator",
                        LastName = "GroupSpace"
                    };

                    await userManager.CreateAsync(administrator, "Abc!12345");
                    await userManager.CreateAsync(dummy, "Abc!12345");

                    context.Roles.AddRange
                    (
                       new IdentityRole { Id = "Beheerder", Name = "Beheerder", NormalizedName = "BEHEERDER" },
                       new IdentityRole { Id = "Gebruiker", Name = "Gebruiker", NormalizedName = "GEBRUIKER"}
                    );
                    context.SaveChanges();

                    string id = administrator.Id;

                    context.UserRoles.AddRange
                        (
                            new IdentityUserRole<string> { RoleId = "Gebruiker", UserId = administrator.Id },
                            new IdentityUserRole<string> { RoleId = "Beheerder", UserId = administrator.Id }
                        );
                    context.SaveChanges();

                }
                if (!context.Group.Any())           // Als er geen groepen aanwezig zijn => Voeg groepen toe
                {
                    context.Group.AddRange(
                        new Group { Name = "?", Description = "?", Started = DateTime.MinValue, Ended = DateTime.Now},
                        new Group { Name = "Testgroep", Description = "Testgroep voor de programmeurs", Started = DateTime.Now, Ended = DateTime.MaxValue });
                    context.SaveChanges();
                }

                if (!context.Message.Any())
                {
                    GroupSpace2022User dummyUser = context.Users.FirstOrDefault(u => u.UserName == "dummy");
                    Group dummy = context.Group.Where(g => g.Name == "?").First();
                    context.Message.AddRange(
                        new Message { Title = "-", Content = "-", GroupId = dummy.Id, SenderId = dummyUser.Id },
                        new Message { Title = "-", Content = "-", GroupId = dummy.Id + 1, SenderId = dummyUser.Id});
                    context.SaveChanges();
                }


                if (!context.Category.Any())
                {
                    Category dummy = new Category { Name = "?", Description = "?" };
                    context.Category.Add(dummy);
                    context.SaveChanges();
                }

                if (!context.Media.Any())
                {
                    List<Category> DummyCategories = new List<Category>();
                    DummyCategories.Add(context.Category.Where(c => c.Name == "?").First());
                    context.Media.AddRange(
                        new Media { Name = "?", Description = "?", Categories = DummyCategories });
                    context.SaveChanges();
                }

                return null;
            }
        }
    }
}
