namespace BookingData.Models
{
    public interface IEventRepository
    {
        IQueryable<Event> Events { get; }
        void CreateEvent(Event e);
        void SaveEvent(Event e);
        void DeleteEvent(Event e);
    }
}
