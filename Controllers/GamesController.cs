using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReactGamesListAPI.Data.Interfaces;
using ReactGamesListAPI.Dtos;

namespace ReactGamesListAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController(IGameRepo repository, IMapper mapper) : ControllerBase
{
    private readonly IGameRepo _repository = repository;
    private readonly IMapper _mapper =  mapper;

    [HttpGet]
    public async Task<IActionResult> GetAllGames()
    {
        var games = await  _repository.GetAllGamesAsync();
        
        return Ok(_mapper.Map<IEnumerable<ReadGameDto>>(games));
        
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGameById(int id)
    {
        var game = await _repository.GetGameByIdAsync(id);
        if  (game == null)
            return NotFound();
        
        return Ok(_mapper.Map<ReadGameDto>(game));
    }

    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetGameByUsername(string username)
    {
        var games = await _repository.GetGamesByUsername(username);
        return Ok(_mapper.Map<IEnumerable<ReadGameDto>>(games));
    }
    
}