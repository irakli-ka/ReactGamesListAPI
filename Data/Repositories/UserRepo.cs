using Microsoft.EntityFrameworkCore;
using ReactGamesListAPI.Data.Interfaces;
using ReactGamesListAPI.Models;

namespace ReactGamesListAPI.Data.Repositories;

public class UserRepo(AppDbContext context, IGameRepo gameRepo) : IUserRepo
{
    private readonly AppDbContext _context = context;
    private readonly IGameRepo _gameRepo = gameRepo;
    
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
    }

    public async Task AddGameToUserList(int userId, Game game)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }
        //CreateGameAsync() handles null/db duplicate checks
        var gameEntity = await _gameRepo.CreateGameAsync(game);

        var inUserGameList = await _context.UserGameLists.AnyAsync(ugl => ugl.UserId == userId && 
                                                                       ugl.GameId == gameEntity.RawgId);
        if (inUserGameList)
            throw new InvalidOperationException(
                $"Game with ID {gameEntity.RawgId} and {gameEntity.Name} is already in the user's game list.");
        
        await _context.UserGameLists.AddAsync(new UserGameList
        {
            UserId = userId,
            User = user,
            GameId = gameEntity.RawgId,
            Game = gameEntity
        });
        
        await _context.SaveChangesAsync();
    }

    public async Task RemoveGameFromUserList(int userId, int rawgId)
    {
        
        if (await _context.UserGameLists.AnyAsync(ugl => ugl.GameId == rawgId && ugl.UserId == userId))
        {
            await _context.UserGameLists
                .Where(ugl => ugl.GameId == rawgId && ugl.UserId == userId)
                .ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Game with ID {rawgId} not found in user with ID {userId}'s game list.");
        }
    }
}