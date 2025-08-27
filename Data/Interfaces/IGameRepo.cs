using ReactGamesListAPI.Models;

namespace ReactGamesListAPI.Data.Interfaces;

public interface IGameRepo
{
    Task<IEnumerable<Game>> GetAllGamesAsync();
    Task<Game?> GetGameByIdAsync(int rawgId);
    Task<Game> CreateGameAsync(Game game);
    Task<IEnumerable<Game>> GetGamesByUsername(string username);

}