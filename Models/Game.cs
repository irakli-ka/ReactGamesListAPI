using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactGamesListAPI.Models;

public class Game
{   
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int RawgId {get;set;}
    public required string Name {get;set;}
    public required string Release {get;set;}
    public required string Rating {get;set;}
    public required string BackgroundImage {get;set;}
    public required List<string> Genres {get;set;}
    public ICollection<UserGameList> UserLists { get; set; } = new List<UserGameList>();

}