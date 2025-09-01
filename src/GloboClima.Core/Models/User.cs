namespace GloboClima.Core.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<string> FavoriteCities { get; set; } = new();
        public List<string> FavoriteCountries { get; set; } = new();
    }
}
