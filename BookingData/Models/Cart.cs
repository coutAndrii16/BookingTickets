namespace BookingData.Models
{
    public class Cart
    {
        public List<int> EventIds { get; set; } = new();

        public void AddItem(int eventId)
        {
            if (!EventIds.Contains(eventId))
                EventIds.Add(eventId);
        }

        public void RemoveItem(int eventId)
        {
            EventIds.Remove(eventId);
        }
    }
}
