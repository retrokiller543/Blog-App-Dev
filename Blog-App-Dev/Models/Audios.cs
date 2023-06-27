using System.ComponentModel.DataAnnotations;

namespace Blog_App_Dev.Models
{
    public class Audios
    {
        [Key]
        [Required]
        public int ID { get; set; }
        public string Language { get; set; }
        public string Region { get; set; }

        public int? StreamingServiceID { get; set; }
        public virtual StreamingService? StreamingService { get; set; }
    }
}
