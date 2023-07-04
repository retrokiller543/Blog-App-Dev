using Ganss.Xss;
using Markdig;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_App_Dev.Models
{
    public class Comment
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime DatePosted { get; set; } = DateTime.Now;
        public string? UserID { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public int? PostID { get; set; }
        public virtual BlogPost? Post { get; set; }

        [NotMapped]
        public string FormattedContent
        {
            get
            {
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                var html = Markdown.ToHtml(Content ?? "", pipeline);

                var sanitizer = new HtmlSanitizer();
                return sanitizer.Sanitize(html);
            }
        }
    }
}
