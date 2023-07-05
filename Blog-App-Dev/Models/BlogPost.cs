using Ganss.Xss;
using Markdig;
using Markdig.Prism;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_App_Dev.Models
{
  public class BlogPost
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
    public ICollection<Comment>? Comments { get; set; }

    [NotMapped]
    public string FormattedContent
    {
      get
      {
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions()
          .UsePrism(new PrismOptions
          {
            UseLineNumbers = true,
            UseDownloadButton = true,
          })
          .UseBootstrap()
          .UseEmojiAndSmiley()
          .UseMathematics()
          .Build();
        var html = Markdown.ToHtml(Content ?? "", pipeline);

        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedAttributes.Add("id");
        sanitizer.AllowedAttributes.Add("data-download-link");
        sanitizer.AllowedAttributes.Add("data-extension");
        return sanitizer.Sanitize(html);
      }
    }
    [NotMapped]
    public string? Excerpt { get; set; }
  }
}
