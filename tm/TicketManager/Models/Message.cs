using System.ComponentModel.DataAnnotations;

namespace TicketManager.Models;

public class Message
{
    [Key]
    public int Id { get; set; }

    public string SenderName { get; set; }

    public string RecipientName { get; set; }

    public string Body { get; set; }

    public DateTime Date { get; set; }

    public string SenderId { get; set; }
    public string RecipientId { get; set; }
    public int ProjectId { get; set; }
}
