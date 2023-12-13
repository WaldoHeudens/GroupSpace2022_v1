using GroupSpace2022.Areas.Identity.Data;
using GroupSpace2022.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GroupSpace2022.Data;

public class GroupSpace2022Context : IdentityDbContext<GroupSpace2022User>
{
    public GroupSpace2022Context(DbContextOptions<GroupSpace2022Context> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public DbSet<GroupSpace2022.Models.Group> Group { get; set; } = default!;

    public DbSet<GroupSpace2022.Models.Message> Message { get; set; }

    public DbSet<GroupSpace2022.Models.MessageDestination> MessageDestinations { get; set; }

    public DbSet<GroupSpace2022.Models.Category> Category { get; set; }

    public DbSet<GroupSpace2022.Models.Media> Media { get; set; }

    public DbSet<GroupSpace2022.Models.MediaType> MediaType { get; set; }

    public DbSet<GroupSpace2022.Models.Token> Token { get; set; }

    public DbSet<GroupSpace2022.Models.UserGroup> UserGroup { get; set; }

    public DbSet<GroupSpace2022.Models.Language> Language { get; set; }

    public DbSet<GroupSpace2022.Models.Parameter> Parameters { get; set; }

}
