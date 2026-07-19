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

    [HttpPost("signup")]
    public async Task<ActionResult> SignUp([FromBody] CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("username and password are required");

        if (await repository.GetUserByUsernameAsync(dto.Username) != null)
            return BadRequest("Username already taken");

        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Username = dto.Username,
            Password = ""
        };

        user.Password = hasher.HashPassword(user, dto.Password);
        await repository.AddUserAsync(user);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("invalid username or password");

        var user = await repository.GetUserByUsernameAsync(dto.Username);
        if (user == null)
            return BadRequest("invalid username or password");

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.Password, dto.Password);

        if (result == PasswordVerificationResult.Failed)
            return BadRequest("invalid username or password");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
        };

        var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return Ok(mapper.Map<ReadUserDto>(user));
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

        var user = await repository.GetUserByIdAsync(int.Parse(claimUserId));
        if (user == null)
            return Unauthorized();

        return Ok(mapper.Map<ReadUserDto>(user));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUserSelf()
    {
        var claimUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var claimUserName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

        
        if (claimUserId == null || claimUserName == null)
            return Unauthorized();
        
        var user = await repository.GetUserByIdAsync(int.Parse(claimUserId));

        if (user == null || user.Username != claimUserName)
            return Unauthorized();
    
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await repository.DeleteUserAsync(int.Parse(claimUserId));
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadUserDto>>> GetAllUsers()
    {
        var users = await repository.GetAllUsersAsync();
        return Ok(mapper.Map<IEnumerable<ReadUserDto>>(users));
    }

    [HttpGet("id/{id}")]
    public async Task<ActionResult<ReadUserDto>> GetUserById(int id)
    {
        var user = await repository.GetUserByIdAsync(id);
        if (user == null)
            return NotFound($"User with id {id} not found");
        return Ok(mapper.Map<ReadUserDto>(user));
    }

    [HttpGet("username/{username}")]
    public async Task<ActionResult<ReadUserDto>> GetUserByUsername(string username)
    {
        var user = await repository.GetUserByUsernameAsync(username);
        if (user == null)
            return NotFound($"User with username {username} not found");
        return Ok(mapper.Map<ReadUserDto>(user));
    }

    [HttpPost("games")]
    public async Task<ActionResult> AddGameToUserList(CreateGameDto gameDto)
    {
        var claimUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (claimUserId == null)
            return Unauthorized();
        
        var game  = mapper.Map<Game>(gameDto);
        await repository.AddGameToUserList(int.Parse(claimUserId), game);
        return Ok();
    }
    
    [HttpDelete("games")]
    public async Task<ActionResult> DeleteGameFromUserList(int gameId)
    {
        var claimUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (claimUserId == null)
            return Unauthorized();
        
        await repository.RemoveGameFromUserList(int.Parse(claimUserId), gameId);
        return Ok();
    }
}