namespace Giftee.Web.Models;

public class GiftForm
{
    public int? Age { get; set; }
    public decimal? Budget { get; set; }
    public string? FavouriteArtist { get; set; }
    public string? FavouriteShow { get; set; }
    public string? Personality { get; set; }
    public string? Interests { get; set; } // comma separated
}

public class Gift
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string[] Tags { get; set; } = [];
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public string Description { get; set; } = "";
}

public class GiftMatch
{
    public Gift Gift { get; set; } = new();
    public int Score { get; set; }
    public List<string> Reasons { get; set; } = [];
}