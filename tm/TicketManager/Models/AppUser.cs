using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TicketManager.Models;

public class AppUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public string AssignedRole { get; set; }

    public string ProfilePicture { get; set; }

    public int NotificationCount { get; set; }
    public int MessageCount { get; set; }

    public ICollection<Ticket> Tickets { get; set; }
    public ICollection<Message> Messages { get; set; }
}
