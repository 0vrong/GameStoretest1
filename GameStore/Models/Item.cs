namespace GameStore.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}