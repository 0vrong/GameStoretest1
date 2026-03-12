namespace GameStore.Models
{
    public class MarketListing
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int SellerId { get; set; }
        public User Seller { get; set; }

        public int Price { get; set; }
        public string Status { get; set; }
    }
}