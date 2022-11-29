using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GroupSpace2022.Models;

namespace GroupSpace2022.Data
{
    public class GroupSpace2022Context : DbContext
    {
        public GroupSpace2022Context (DbContextOptions<GroupSpace2022Context> options)
            : base(options)
        {
        }

        public DbSet<GroupSpace2022.Models.Group> Group { get; set; } = default!;
    }
}
