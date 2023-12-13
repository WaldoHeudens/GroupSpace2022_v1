using System.ComponentModel.DataAnnotations;

namespace GroupSpace2022.Models
{
    public class Parameter
    {
        [Key]
        [Display (Name="Naam")]
        public string Name { get; set; }

        [Display(Name = "Waarde")]
        public string Value { get; set; }

        [Display(Name = "Omschrijving")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string Destination { get; set; }
        public DateTime Added { get; set; } = DateTime.Now;
        public DateTime OutOfUse { get; set; } = DateTime.MaxValue;
    }
}
