using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupSpace2022.Models
{
    public class Media
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Naam")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Omschrijving")]
        public string Description { get; set; }

        [Display(Name = "Toegevoegd")]
        public DateTime Added { get; set; }

        [Display(Name = "Verwijderd")]
        public DateTime Deleted { get; set; }

        [Display(Name = "Type")]
        public int TypeId { get; set; }

        [Display(Name = "Categorieën")]
        [NotMapped]
        public List<int> CategoryIds { get; set; }

        [Display(Name = "Type")]
        public MediaType? Type { get; set; }

        [Display(Name = "Categorieën")]
        public List<Category>? Categories { set; get; }

    }

    public class MediaType
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Naam")]
        public string Name { get; set; }

        [Required]
        public string Denominator { get; set; }

        public DateTime Deleted { get; set; }
    }

}
