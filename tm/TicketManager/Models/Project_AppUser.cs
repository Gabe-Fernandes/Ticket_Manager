using System.ComponentModel.DataAnnotations;

namespace TicketManager.Models;

public class Project_AppUsers
{
    [Key]
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string AppUserId { get; set; }

    public bool Approved { get; set; }
}
