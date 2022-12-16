using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GroupSpace2022.Models;
using Microsoft.AspNetCore.Identity;

namespace GroupSpace2022.Areas.Identity.Data;

// Add profile data for application users by adding properties to the GroupSpace2022User class
public class GroupSpace2022User : IdentityUser
{
    [Required]
    [Display(Name = "Voornaam")]
    public string FirstName { get; set; } = "?";

    [Required]
    [Display(Name = "Familienaam")]
    public string LastName { get; set; } = "?";

    [Display(Name = "Verwijderd")]
    public DateTime Deleted { get; set; } = DateTime.MaxValue;

    [ForeignKey("Group")]
    public int? ActualGroupId { get; set; }  // Nullable, to avoid cascading key conflicts with GroupUser
    public Group? ActualGroup { get; set; }
    public List<UserGroup>? Groups { get; set; }
}

