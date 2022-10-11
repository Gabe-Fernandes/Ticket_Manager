using System.ComponentModel.DataAnnotations;

namespace TicketManager.Models;

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20, ErrorMessage = "Maximum length reached")]
    public string Title { get; set; }

    [Required]
    [StringLength(400, ErrorMessage = "Maximum length reached")]
    public string Description { get; set; }

    [Required]
    public string Status { get; set; }

    [Required]
    public string PriorityLevel { get; set; }

    [Required]
    public DateTime TempDate { get; set; }

    [Required]
    public string RecipientName { get; set; }
    public string RecipientPfp { get; set; }
    public string RecipientId { get; set; }

    public string SenderName { get; set; }
    public string SenderPfp { get; set; }
    public string SenderId { get; set; }

    public string StartDate { get; set; }
    public string EndDate { get; set; }

    public string DetailsBtnId { get; set; }
    public string TableRowId { get; set; }
    public int ProjectId { get; set; }
}
