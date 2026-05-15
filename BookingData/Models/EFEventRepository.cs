namespace BookingData.Models
{
    public class EFEventRepository : IEventRepository
    {
        private readonly EventDbContext context;

        public EFEventRepository(EventDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Event> Events => context.Events;

        public void CreateEvent(Event e)
        {
            context.Add(e);
            context.SaveChanges();
        }

        public void DeleteEvent(Event e)
        {
            context.Remove(e);
            context.SaveChanges();
        }

        public void SaveEvent(Event e)
        {
            context.SaveChanges();
        }
    }
}
