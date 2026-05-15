using Microsoft.EntityFrameworkCore;

namespace BookingData.Models
{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options)
            : base(options) { }

        public DbSet<Event> Events => Set<Event>();
        public DbSet<Ticket> Tickets => Set<Ticket>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
