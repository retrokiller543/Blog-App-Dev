using System.ComponentModel.DataAnnotations;

namespace Blog_App_Dev.Models
{
    public class StreamingService
    {
        [Key]
        [Required]
        public int ID { get; set; }
        public string Type { get; set; } = "";
        public string Service { get; set; } = "";
        public string Quality { get; set; } = "";
        public string AddOn { get; set; } = "";
        public string Link { get; set; } = "";
        public string watchLink { get; set; } = "";
        public ICollection<Audios>? Audios { get; set; }
        public ICollection<Subtitles>? Subtitles { get; set; }

        public int? RegionInfoID { get; set; }
        public virtual RegionInfo? RegionInfo { get; set; }
    }
}
