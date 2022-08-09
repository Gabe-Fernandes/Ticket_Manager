using System.ComponentModel.DataAnnotations;

namespace TicketManager.Models;

public class Message
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Body { get; set; }

    public DateTime Date { get; set; }
}
