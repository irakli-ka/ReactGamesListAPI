using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReactGamesListAPI.Data.Interfaces;
using ReactGamesListAPI.Dtos;

namespace ReactGamesListAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController(IGameRepo repository, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllGames()
    {
        var games = await  repository.GetAllGamesAsync();
        
        return Ok(mapper.Map<IEnumerable<ReadGameDto>>(games));
        
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGameById(int id)
    {
        var game = await repository.GetGameByIdAsync(id);
        if  (game == null)
            return NotFound();
        
        return Ok(mapper.Map<ReadGameDto>(game));
    }

    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetGameByUsername(string username)
    {
        var games = await repository.GetGamesByUsername(username);
        return Ok(mapper.Map<IEnumerable<ReadGameDto>>(games));
    }
    
}