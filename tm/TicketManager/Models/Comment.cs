using System.ComponentModel.DataAnnotations;

namespace TicketManager.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Pfp { get; set; }

    public string Body { get; set; }

    public string Date { get; set; }

    public string TableRowId { get; set; }
    public int TicketId { get; set; }
}
