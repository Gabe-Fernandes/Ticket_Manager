using System.ComponentModel.DataAnnotations;

namespace TicketManager.Models;

public class Message
{
    [Key]
    public int Id { get; set; }

    public string SenderName { get; set; }

    public string ReceiverName { get; set; }

    public string Body { get; set; }

    public DateTime Date { get; set; }

    public string From { get; set; }

    public string To { get; set; }

    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }
}
