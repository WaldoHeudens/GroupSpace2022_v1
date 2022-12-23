using GroupSpace2022.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace GroupSpace2022.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Display(Name = "Naam")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Omschrijving")]
        public string Description { get; set; }

        [Display(Name = "Gestart")]
        [DataType(DataType.Date)]
        public DateTime Started { get; set; } = DateTime.Now;

        [Display(Name = "Beëindigd")]
        [DataType(DataType.Date)]
        public DateTime Ended { get; set; } = DateTime.MaxValue;

        public string StartedById { get; set; }

        public string EndedById { get; set; }

        public DateTime Deleted { get; set; } = DateTime.MaxValue;

        public List<UserGroup>? UserGroups { get; set; }

    }


    public class UserGroup
    {
        public int Id { get; set; }

        [ForeignKey("Group")]
        [Display (Name = "Groep")]
        public int GroupId { get; set; } = 1;
        public Group? Group { get; set; }

        [ForeignKey("GroupSpace2022User")]
        [Display(Name = "Gebruiker")]
        public string UserId { get; set; } = "Dummy";
        public GroupSpace2022User? User { get; set; }
        [Display(Name = "Lid sinds")]
        public DateTime Added { get; set; } = DateTime.Now;
        [Display(Name = "Lid tot")]
        public DateTime Left { get; set; } = DateTime.MaxValue;
        [Display(Name = "Host sinds")]
        public DateTime BecameHost { get; set; } = DateTime.MinValue;
        [Display(Name = "Host tot")]
        public DateTime NoHostAnymore { get; set; } = DateTime.MaxValue;
    }

    public class GroupViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Naam")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Started")]
        public DateTime Started { get; set; }
        [Display(Name = "StartedBy")]
        public string StartedBy { get; set; }
        [Display(Name = "Hosts")]
        public List<string> Hosts { get; set; }
        [Display(Name = "Members")]
        public List<string> Members { get; set; }
        [Display(Name = "isHost")]
        public bool isHost { get; set; }
    }

    public class InviteViewModel
    {
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }
        [Display(Name = "Surname")]
        public string Surname { get; set; }
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        public int GroupId { get; set; }
    }

    public class MemberViewModel
    {
        public string UserId { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Added")]
        public DateTime Added { get; set; }
        [Display(Name = "Host ?")]
        public Boolean isHost { get; set; }

    }
}
