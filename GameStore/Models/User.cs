namespace GameStore.Models
{
    public class User
    { 
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public decimal Balace { get; set; } = 0;
    }
}
