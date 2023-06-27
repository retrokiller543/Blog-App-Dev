using System.ComponentModel.DataAnnotations;

namespace Blog_App_Dev.Models
{
    public class Subtitles
    {
        [Key]
        [Required]
        public int ID { get; set; }
        public string? Language { get; set; }
        public string? Region { get; set; }
        public bool? ClosedCaptions { get; set; } = false;

        public int? StreamingServiceID { get; set; }
        public virtual StreamingService? StreamingService { get; set; }
    }
}
