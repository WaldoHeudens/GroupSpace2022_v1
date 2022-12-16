using System.ComponentModel.DataAnnotations;

namespace GroupSpace2022.Models
{
    public class Token
    {
        public Guid Id { get; set; }
        public string Connection { get; set; }
        public int ConnectedId { get; set; }
        public DateTime Added {get; set;}
        public string AddedBy { get; set; }

    }
}
