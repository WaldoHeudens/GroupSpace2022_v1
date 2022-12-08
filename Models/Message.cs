using GroupSpace2022.Areas.Identity.Data;
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

        [ForeignKey("GroupSpace2022User")]
        [Display (Name = "Verzonden door")]
        public string SenderId { get; set; }

        public DateTime Deleted { get; set; } = DateTime.MaxValue;

        public GroupSpace2022User? Sender { get; set; }

    }
}
