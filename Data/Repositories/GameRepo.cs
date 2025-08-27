using Microsoft.EntityFrameworkCore;
using ReactGamesListAPI.Data.Interfaces;
using ReactGamesListAPI.Models;

namespace ReactGamesListAPI.Data.Repositories;

public class GameRepo(AppDbContext context) : IGameRepo
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Game>> GetAllGamesAsync()
    {
        return await _context.Games.ToListAsync();
    }

    public async Task<Game?> GetGameByIdAsync(int rawgId)
    {
        return await _context.Games.FindAsync(rawgId);
    }
    
    public async Task<Game> CreateGameAsync(Game game)
    {
        if (game == null)
        {
            throw new ArgumentNullException(nameof(game), "Game cannot be null");
        }
        
        var existingGame = await _context.Games.FirstOrDefaultAsync(g => g.RawgId == game.RawgId);
        if (existingGame != null)
        {
            return existingGame;
        }

        var entry = await _context.Games.AddAsync(game);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }


    public async Task<IEnumerable<Game>> GetGamesByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be null or empty", nameof(username));
        }
    
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            //change?
            // throw new InvalidOperationException($"User with username '{username}' not found.");
        }
    
        var games = await _context.UserGameLists
            .Where(ugl => ugl.UserId == user.Id)
            .Select(ugl => ugl.Game)
            .ToListAsync();
        
        return games;
    }
}

    