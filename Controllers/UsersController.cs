using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReactGamesListAPI.Data.Interfaces;
using ReactGamesListAPI.Dtos;
using ReactGamesListAPI.Models;

namespace ReactGamesListAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserRepo repository, IMapper mapper) : ControllerBase
{
    private readonly IUserRepo _repository = repository;
    private readonly IMapper _mapper = mapper;

    [HttpPost("signup")]
    public async Task<ActionResult> SignUp(string username, string password)
    {
        if (await _repository.GetUserByUsernameAsync(username)  != null)
            return BadRequest("Username already taken");

        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Username = username,
            Password = ""
        };
        
        user.Password = hasher.HashPassword(user, password);
        await _repository.AddUserAsync(user);
        
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(string username, string password)
    {
        var user  = await _repository.GetUserByUsernameAsync(username);
        if (user == null)
            return BadRequest("invalid username or  password");
       
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.Password, password);
        
        if (result == PasswordVerificationResult.Failed)
            return BadRequest("invalid username or  password");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
        };
        
        var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return Ok(_mapper.Map<ReadUserDto>(user));
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var claimUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var claimUserName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

        
        if (claimUserId == null || claimUserName == null)
            return Unauthorized("Not Logged In");

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }
    
    [HttpGet("me")]
    public async Task<ActionResult> GetCurrentUser()
    {
        var claimUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var claimUserName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
    
        if (claimUserId == null || claimUserName == null)
            return Unauthorized();

        var user = await _repository.GetUserByIdAsync(int.Parse(claimUserId));
        if (user == null)
            return Unauthorized();

        return Ok(_mapper.Map<ReadUserDto>(user));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUserSelf()
    {
        var claimUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var claimUserName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

        
        if (claimUserId == null || claimUserName == null)
            return Unauthorized();
        
        var user = await _repository.GetUserByIdAsync(int.Parse(claimUserId));

        if (user == null || user.Username != claimUserName)
            return Unauthorized();
    
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await _repository.DeleteUserAsync(int.Parse(claimUserId));
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadUserDto>>> GetAllUsers()
    {
        var users = await _repository.GetAllUsersAsync();
        return Ok(_mapper.Map<IEnumerable<ReadUserDto>>(users));
    }

    [HttpGet("id/{id}")]
    public async Task<ActionResult<ReadUserDto>> GetUserById(int id)
    {
        var user = await _repository.GetUserByIdAsync(id);
        if (user == null)
            return NotFound($"User with id {id} not found");
        return Ok(_mapper.Map<ReadUserDto>(user));
    }

    [HttpGet("username/{username}")]
    public async Task<ActionResult<ReadUserDto>> GetUserByUsername(string username)
    {
        var user = await _repository.GetUserByUsernameAsync(username);
        if (user == null)
            return NotFound($"User with username {username} not found");
        return Ok(_mapper.Map<ReadUserDto>(user));
    }

    [HttpPost("games")]
    public async Task<ActionResult> AddGameToUserList(CreateGameDto gameDto)
    {
        var claimUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (claimUserId == null)
            return Unauthorized();
        
        var game  = _mapper.Map<Game>(gameDto);
        await _repository.AddGameToUserList(int.Parse(claimUserId), game);
        return Ok();
    }
    
    [HttpDelete("games")]
    public async Task<ActionResult> DeleteGameFromUserList(int gameId)
    {
        var claimUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (claimUserId == null)
            return Unauthorized();
        
        await _repository.RemoveGameFromUserList(int.Parse(claimUserId), gameId);
        return Ok();
    }
}