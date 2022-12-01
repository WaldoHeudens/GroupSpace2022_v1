using System.ComponentModel.DataAnnotations;

namespace GroupSpace2022.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Categorie")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Omschrijving")]
        public string Description { get; set; }

        [Display(Name = "Media")]
        public List<Media>? Medias { get; set; }

        public DateTime Deleted { get; set; } = DateTime.MaxValue;
    }
}
