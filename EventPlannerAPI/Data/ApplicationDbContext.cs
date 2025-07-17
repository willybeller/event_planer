using EventPlannerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlannerAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventParticipant> EventParticipants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Name).IsRequired().HasMaxLength(255);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.Password).IsRequired();
        });

        // Event configuration
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnType("TEXT");
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.Time).IsRequired();
            
            // Foreign key relationship
            entity.HasOne(e => e.Creator)
                  .WithMany(u => u.CreatedEvents)
                  .HasForeignKey(e => e.CreatorId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // EventParticipant configuration
        modelBuilder.Entity<EventParticipant>(entity =>
        {
            entity.HasKey(ep => ep.Id);
            entity.Property(ep => ep.Email).IsRequired().HasMaxLength(255);
            entity.Property(ep => ep.RsvpStatus).IsRequired().HasMaxLength(50);
            
            // Unique constraint: one email per event
            entity.HasIndex(ep => new { ep.EventId, ep.Email }).IsUnique();
            
            // Foreign key relationships
            entity.HasOne(ep => ep.Event)
                  .WithMany(e => e.Participants)
                  .HasForeignKey(ep => ep.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(ep => ep.User)
                  .WithMany(u => u.Participations)
                  .HasForeignKey(ep => ep.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
