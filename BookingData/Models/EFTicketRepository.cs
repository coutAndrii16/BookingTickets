namespace BookingData.Models
{
    public class EFTicketRepository : ITicketRepository
    {
        private readonly EventDbContext context;

        public EFTicketRepository(EventDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Ticket> Tickets => context.Tickets;

        public void CreateTicket(Ticket t)
        {
            context.Add(t);
            context.SaveChanges();
        }

        public void DeleteTicket(Ticket t)
        {
            context.Remove(t);
            context.SaveChanges();
        }

        public void SaveTicket(Ticket t)
        {
            context.SaveChanges();
        }
    }
}
