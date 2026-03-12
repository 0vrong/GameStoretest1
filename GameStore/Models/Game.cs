namespace GameStore.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
    }
}
