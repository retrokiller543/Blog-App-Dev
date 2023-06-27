using System.ComponentModel.DataAnnotations;

namespace Blog_App_Dev.Models
{
    public class RegionInfo
    {
        [Key]
        [Required]
        public int RegionID { get; set; }
        public string RegionName { get; set; } = "";

        public ICollection<StreamingService>? StreamingServices { get; set; }
    }
}
