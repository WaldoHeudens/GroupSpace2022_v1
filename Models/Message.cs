using GroupSpace2022.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupSpace2022.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Content")]
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Created")]
        public DateTime Created { get; set; }

        [ForeignKey("GroupSpace2022User")]
        public string SenderId { get; set; } = "-";
        public GroupSpace2022User? Sender { get; set; }
        [Display(Name = "Media")]
        public List<Media>? Media { get; set; }
        [Display(Name = "Destinations")]
        public List<MessageDestination>? Destinations { get; set; }
    }

    public class MessageDestination
    {
        public int Id { get; set; }
        [ForeignKey("Message")]
        [Display(Name = "Message")]
        public int MessageId { get; set; }
        [ForeignKey("GroupSpace2022User")]
        [Display(Name = "Receiver")]
        public string ReceiverId { get; set; }
        [Display(Name = "Received")]
        public DateTime Received { get; set; }
        [Display(Name = "Read")]
        public DateTime Read { get; set; }
        [Display(Name = "Deleted")]
        public DateTime Deleted { get; set; }
        [Display(Name = "Message")]
        public Message? Message { get; set; }
        [Display(Name = "Receiver")]
        public GroupSpace2022User? Receiver { get; set; }
    }

    public class MessageViewModel
    {
        public int MessageId { get; set; }
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Message")]
        public string Content { get; set; }
        [Display(Name = "Attachments")]
        public List<Media>? Attachments { get; set; }
        [Display(Name = "Groups")]
        public List<int>? GroupIds { get; set; }
        [Display(Name = "Groups")]
        public List<Group>? Groups { get; set; }
        [Display(Name = "Destinies")]
        public List<string>? DestinyIds { get; set; }
        [Display(Name = "Destinies")]
        public List<GroupSpace2022User>? Destinies { get; set; }

        [Display(Name = "Sent")]
        public DateTime Sent { get; set; }
    }


    public class MessageIndexViewModel
    {
        public string MessageFilter { get; set; }
        public string TitleFilter { get; set; }
        public Paginas<MessageViewModel> Messages { get; set; }
        public SelectList ModesToSelect { get; set; }
        public Dictionary<int, SelectList> UsersTotSelect { get; set; }  // List of users per group
        public List<string> Emailaddresses { get; set; }
        public string SelectedMode { get; set; }
    }
}
