using ReactGamesListAPI.Models;

namespace ReactGamesListAPI.Data.Interfaces;

public interface IUserRepo
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task AddUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task AddGameToUserList(int userId, Game game);
    Task RemoveGameFromUserList(int userId, int gameId);
}