namespace ReactGamesListAPI.Dtos;

public class ReadGameDto
{
    public required int RawgId {get;set;}
    public required string Name {get;set;}
    public required string Release {get;set;}
    public required string Rating {get;set;}
    public required string BackgroundImage {get;set;}
    public required List<string> Genres {get;set;}
}