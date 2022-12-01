using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupSpace2022.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Display (Name = "Titel")]
        [Required]
        public string Title { get; set; }

        [Required]
        [Display (Name = "Boodschap")]
        public string Content { get; set; }

        [Display (Name ="Verzonden")]
        [DataType (DataType.DateTime)]
        public DateTime Sent { get; set; } = DateTime.Now;


        [ForeignKey("Group")]
        [Display (Name = "Groep")]
        public int GroupId { get; set; }

        [Display(Name = "Groep")]
        public Group? Group { get; set; }

        public DateTime Deleted { get; set; } = DateTime.MaxValue;

    }
}
