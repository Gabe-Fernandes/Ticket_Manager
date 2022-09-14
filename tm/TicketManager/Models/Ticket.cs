using System.ComponentModel.DataAnnotations;

namespace TicketManager.Models;

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20, ErrorMessage = "Maximum length reached")]
    public string Name { get; set; }

    [Required]
    [StringLength(400, ErrorMessage = "Maximum length reached")]
    public string Description { get; set; }

    [Required]
    public string Status { get; set; }

    [Required]
    public string PriorityLevel { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public string AssignedTo { get; set; }
    public string AssignedFrom { get; set; }

    public ICollection<Comment> Comments { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }
}
