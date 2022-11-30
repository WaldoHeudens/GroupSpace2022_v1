using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

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

        [Required]
        [Display(Name = "Toegevoegd")]
        [DataType(DataType.DateTime)]
        public DateTime Added { get; set; } = DateTime.Now;

        [NotMapped]
        public List<int>? CategoryIds { get; set; }

        public List<Category>? Categories { get; set; }
    }
}
