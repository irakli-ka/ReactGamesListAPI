using AutoMapper;
using ReactGamesListAPI.Dtos;
using ReactGamesListAPI.Models;

namespace ReactGamesListAPI.Profiles;

public class UsersProfile : Profile
{
    public UsersProfile()
    {
        CreateMap<User, ReadUserDto>();
        CreateMap<CreateUserDto, User>();
    }
} 