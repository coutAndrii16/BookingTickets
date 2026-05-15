namespace BookingData.Models
{
    public interface ITicketRepository
    {
        IQueryable<Ticket> Tickets { get; }
        void CreateTicket(Ticket t);
        void SaveTicket(Ticket t);
        void DeleteTicket(Ticket t);
    }
}
