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
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public int EmployeeCount { get; set; }
    public int NotificationCount { get; set; }
    public int MessageCount { get; set; }
}
