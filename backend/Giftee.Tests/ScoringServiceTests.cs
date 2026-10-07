using Giftee.Api.Models;
using Giftee.Api.Services;
using Xunit;

namespace Giftee.Tests;

public class ScoringServiceTests
{
    private static List<Gift> Sample() =>
    [
        new() { Name = "Headphones", Category = "Tech", Tags = ["music", "gadgets"], Personalities = ["introvert"],
                MinAge = 15, MaxAge = 60, MinPrice = 800, MaxPrice = 3500, Description = "Music gear" },
        new() { Name = "Paint Set", Category = "Art", Tags = ["art", "painting"], Personalities = ["creative"],
                MinAge = 10, MaxAge = 80, MinPrice = 250, MaxPrice = 900, Description = "Painting kit" },
        new() { Name = "Kids Puzzle", Category = "Games", Tags = ["games"], Personalities = ["homebody"],
                MinAge = 3, MaxAge = 8, MinPrice = 100, MaxPrice = 300, Description = "Puzzle" },
        new() { Name = "Drake Vinyl", Category = "Music", Tags = ["drake", "rap", "music"], Personalities = ["extrovert"],
                MinAge = 16, MaxAge = 45, MinPrice = 700, MaxPrice = 1800, Description = "Vinyl records" },
        new() { Name = "Friends Mug Set", Category = "Home", Tags = ["central perk", "tv"], Personalities = ["homebody"],
                MinAge = 12, MaxAge = 70, MinPrice = 250, MaxPrice = 800, Description = "Coffee mugs" }
    ];

    [Fact]
    public void MatchingInterest_RanksThatGiftFirst()
    {
        var req = new RecommendRequest { Interests = ["art"] };
        var result = ScoringService.Recommend(Sample(), req);
        Assert.Equal("Paint Set", result[0].Gift.Name);
    }

    [Fact]
    public void AgeOutsideRange_ExcludesGift()
    {
        var req = new RecommendRequest { Age = 30, Interests = ["games"] };
        var result = ScoringService.Recommend(Sample(), req);
        Assert.DoesNotContain(result, m => m.Gift.Name == "Kids Puzzle");
    }

    [Fact]
    public void BudgetBelowMinPrice_ExcludesGift()
    {
        var req = new RecommendRequest { Budget = 500, Interests = ["music"] };
        var result = ScoringService.Recommend(Sample(), req);
        Assert.DoesNotContain(result, m => m.Gift.Name == "Headphones");
    }

    [Fact]
    public void ArtistInTags_RanksThatGiftFirst()
    {
        var req = new RecommendRequest { FavouriteArtist = "Drake" };
        var result = ScoringService.Recommend(Sample(), req);
        Assert.Equal("Drake Vinyl", result[0].Gift.Name);
    }

    [Fact]
    public void ShowInGiftName_RanksThatGiftFirst()
    {
        var req = new RecommendRequest { FavouriteShow = "Friends" };
        var result = ScoringService.Recommend(Sample(), req);
        Assert.Equal("Friends Mug Set", result[0].Gift.Name);
    }

    [Fact]
    public void NoMatches_ReturnsFallbackSuggestions()
    {
        var req = new RecommendRequest { Age = 25, Interests = ["skydiving"] };
        var result = ScoringService.Recommend(Sample(), req);
        Assert.NotEmpty(result);
        Assert.All(result, m => Assert.Equal(0, m.Score));
    }
}