using AutoMapper;
using ReactGamesListAPI.Dtos;
using ReactGamesListAPI.Models;

namespace ReactGamesListAPI.Profiles;

public class GamesProfile : Profile
{
    public GamesProfile()
    {
        CreateMap<Game, ReadGameDto>();
        CreateMap<CreateGameDto, Game>();
    }    
}