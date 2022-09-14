using System.ComponentModel.DataAnnotations;

namespace TicketManager.Models;

public class Project
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20, ErrorMessage = "Maximum length reached")]
    public string Name { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Maximum length reached")]
    public string GitHubLink { get; set; }

    [Required]
    public string StartDate { get; set; }

    [Required]
    public string EndDate { get; set; }

    public string ProjectCode { get; set; }

    public int TeamMemberCount { get; set; }

    public ICollection<Ticket> Tickets { get; set; }
}
