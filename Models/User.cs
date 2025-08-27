namespace ReactGamesListAPI.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public ICollection<UserGameList> Games { get; set; } = new List<UserGameList>();
}