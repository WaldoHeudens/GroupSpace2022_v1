using Microsoft.EntityFrameworkCore;
using GroupSpace2022.Models;

namespace GroupSpace2022.Data
{
    public class SeedDatacontext
    {
        public static void Initialize(System.IServiceProvider serviceProvider)
        {
            using (var context = new GroupSpace2022Context(serviceProvider.GetRequiredService<DbContextOptions<GroupSpace2022Context>>()))
            {
                context.Database.Migrate();
                context.Database.EnsureCreated();   // Zorg dat de databank bestaat

                if (!context.Group.Any())           // Als er geen groepen aanwezig zijn => Voeg groepen toe
                {
                    context.Group.AddRange(
                        new Group { Name = "?", Description = "?", Started = DateTime.MinValue, Ended = DateTime.Now},
                        new Group { Name = "Testgroep", Description = "Testgroep voor de programmeurs", Started = DateTime.Now, Ended = DateTime.MaxValue });
                    context.SaveChanges();
                }

                if (!context.Message.Any())
                {
                    Group dummy = context.Group.Where(g => g.Name == "?").First();
                    context.Message.AddRange(
                        new Message { Title = "-", Content = "-", GroupId = dummy.Id },
                        new Message { Title = "-", Content = "-", GroupId = dummy.Id + 1});
                    context.SaveChanges();
                }
            }
        }
    }
}
