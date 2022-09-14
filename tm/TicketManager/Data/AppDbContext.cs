using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketManager.Models;

namespace TicketManager.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Comment> Comments { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Project_AppUsers> Project_AppUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //builder.Entity<ProjectAppUsers>()
        //    .HasKey(pa => new { pa.ProjectId, pa.AppUserId });
        //builder.Entity<ProjectAppUsers>()
        //    .HasOne(pa => pa.Project)
        //    .WithMany(pa => pa.TeamMembers)
        //    .HasForeignKey(pa => pa.ProjectId);
        //builder.Entity<ProjectAppUsers>()
        //    .HasOne(pa => pa.AppUser)
        //    .WithMany(pa => pa.Projects)
        //    .HasForeignKey(pa => pa.AppUserId);
    }
}
