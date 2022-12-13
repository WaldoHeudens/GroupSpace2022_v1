using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GroupSpace2022.Models.ViewModels
{

    public class UserViewModel
    {
        [Display (Name = "Gebruiker")]
        public string UserName { get; set; }

        [Display(Name = "Voornaam")]
        public string FirstName { get; set; }

        [Display(Name = "Familienaam")]
        public string LastName { get; set; }

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Verwijderd")]
        public bool Deleted { get; set; }


        [Display(Name = "Rollen")]
        public List<String> Roles { get; set; }

    }
}
