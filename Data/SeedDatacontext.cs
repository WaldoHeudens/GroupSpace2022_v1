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
                context.Database.EnsureCreated();   // Zorg dat de databank bestaat
                context.Database.Migrate();

                // Initialisatie van de talen
                if (!context.Language.Any())
                {
                    context.Language.AddRange
                        (
                            new Language() { Id = "-", Name = "-", Cultures = "", IsShown = false},
                            new Language() { Id = "en", Name = "English", Cultures = "UK;US", IsShown = true },
                            new Language() { Id = "fr", Name = "français", Cultures = "BE;FR", IsShown = true },
                            new Language() { Id = "nl", Name = "Nederlands", Cultures = "BE;NL", IsShown = true }
                        );
                    context.SaveChanges();
                }

                Language.Initialize(context);

                // Initialisatie van de rollen
                if (!context.Roles.Any())
                {

                    GroupSpace2022User dummy = new GroupSpace2022User
                    {
                        Email = "?@?.?",
                        EmailConfirmed = true,
                        LockoutEnabled = true,
                        UserName = "dummy",
                        FirstName = "?",
                        LastName = "?",
                        Deleted = DateTime.Now,
                        LanguageId = "-"
                        };
                    GroupSpace2022User admin = new GroupSpace2022User
                    {
                        Email = "admin@Groupspace2022.be",
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        UserName = "Administrator",
                        FirstName = "Administrator",
                        LastName = "GroupSpace",
                        LanguageId = "nl"
                    };

                    await userManager.CreateAsync(admin, "Abc!12345");
                    await userManager.CreateAsync(dummy, "Abc!12345");

                    context.Roles.AddRange
                    (
                       new IdentityRole { Id = "SystemAdministrator", Name = "SystemAdministrator", NormalizedName = "SYSTEMADMINISTRATOR" },
                       new IdentityRole { Id = "UserAdministrator", Name = "UserAdministrator", NormalizedName = "USERADMINISTRATOR" },
                       new IdentityRole { Id = "User", Name = "User", NormalizedName = "USER" }
                    );
                    context.SaveChanges();

                    string id = admin.Id;

                    context.UserRoles.AddRange
                        (
                            new IdentityUserRole<string> { RoleId = "User", UserId = admin.Id },
                            new IdentityUserRole<string> { RoleId = "UserAdministrator", UserId = admin.Id },
                            new IdentityUserRole<string> { RoleId = "SystemAdministrator", UserId = admin.Id }
                        );
                    context.SaveChanges();

                }
                GroupSpace2022User dummyUser = context.Users.FirstOrDefault(u => u.UserName == "dummy");
                GroupSpace2022User administrator = context.Users.FirstOrDefault(u => u.UserName == "Administrator");

                if (!context.Group.Any())           // Als er geen groepen aanwezig zijn => Voeg groepen toe
                {
                    context.Group.AddRange(
                        new Group { Name = "?", Description = "?", Started = DateTime.MinValue, Ended = DateTime.Now, StartedById = dummyUser.Id, EndedById = dummyUser.Id},
                        new Group { Name = "Testgroep", Description = "Testgroep voor de programmeurs", Started = DateTime.Now, Ended = DateTime.MaxValue, StartedById = administrator.Id, EndedById = administrator.Id });
                    context.SaveChanges();
                }

                if (!context.UserGroup.Any())
                {
                    context.UserGroup.AddRange(
                        new UserGroup { UserId = dummyUser.Id, GroupId = 1 });
                    context.SaveChanges();
                }

                if (!context.Message.Any())   // Voeg enkele messages toe
                {
                    Message dummyMessage = new Message { Title = "dummy", Content = "-", Created = DateTime.Now, SenderId = dummyUser.Id };
                    MessageDestination dummymd = new MessageDestination { Deleted = DateTime.MinValue, Message = dummyMessage, Read = DateTime.Now, Received = DateTime.Now, ReceiverId = dummyUser.Id };
                    context.Message.Add(dummyMessage);
                    context.MessageDestinations.Add(dummymd);
                    context.SaveChanges();
                }

                if (!context.MediaType.Any())
                {
                    context.MediaType.AddRange(
                        new MediaType { Name = "-", Denominator = "-", Deleted = DateTime.Now },
                        new MediaType { Name = "Alles", Denominator = "All File (*.*)|*.*|Alle Bestanden", Deleted = DateTime.MaxValue },
                        new MediaType { Name = "Videos", Denominator = "MP4 (*.mpg)|*.mpg|Videos mp4", Deleted = DateTime.MaxValue });
                    context.SaveChanges();
                }


                if (!context.Category.Any())
                {
                    Category dummy = new Category { Name = "?", Description = "?" };
                    context.Category.AddRange(
                        dummy,
                        new Category { Name = "Family Pictures", Description = "All pictures concerning the whole family", Deleted = DateTime.MaxValue },
                        new Category { Name = "Holidays", Description = "All holiday media", Deleted = DateTime.MaxValue });
                    context.SaveChanges();
                }

                if (!context.Media.Any())
                {
                    List<Category> DummyCategories = new List<Category>();
                    DummyCategories.Add(context.Category.Where(c => c.Name == "?").First());
                    context.Media.AddRange(
                        new Media { Name = "?", Description = "?", Categories = DummyCategories, TypeId=1 });
                    context.SaveChanges();
                }

                return null;
            }
        }
    }
}
