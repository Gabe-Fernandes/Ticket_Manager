using System.ComponentModel.DataAnnotations;

namespace TicketManager.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Body { get; set; }

    public DateTime Date { get; set; }

    public int TicketId { get; set; }
}
