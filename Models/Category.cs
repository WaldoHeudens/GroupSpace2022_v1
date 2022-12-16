using System.ComponentModel.DataAnnotations;

namespace GroupSpace2022.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Naam")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Omschrijving")]
        public string Description { get; set; }

        public DateTime Deleted { get; set; }

        public List<Media>? Media { get; set; }
    }
}
