namespace ReactGamesListAPI.Models;

public class UserGameList
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public int GameId { get; set; }
    public required Game Game { get; set; }
    
}