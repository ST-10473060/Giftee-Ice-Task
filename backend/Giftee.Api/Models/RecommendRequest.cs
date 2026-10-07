namespace Giftee.Api.Models;

public class RecommendRequest
{
    public int? Age { get; set; }
    public decimal? Budget { get; set; }
    public string? FavouriteArtist { get; set; }
    public string? FavouriteShow { get; set; }
    public string? Personality { get; set; }
    public List<string> Interests { get; set; } = [];
}

public class GiftMatch
{
    public Gift Gift { get; set; } = new();
    public int Score { get; set; }
    public List<string> Reasons { get; set; } = [];
}