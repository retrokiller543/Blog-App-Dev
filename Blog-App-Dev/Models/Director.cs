using System.ComponentModel.DataAnnotations;

namespace Blog_App_Dev.Models
{
    public class Director
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public int? MovieID { get; set; }
        public virtual Movie? Movie { get; set; }
    }
}
