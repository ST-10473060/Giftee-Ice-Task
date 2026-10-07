using System.Net.Http.Json;
using Giftee.Web.Models;

namespace Giftee.Web.Services;

public class ApiClient(HttpClient http)
{
    public async Task<(List<GiftMatch>? Matches, string? Error)> RecommendAsync(GiftForm form)
    {
        var payload = new
        {
            age = form.Age,
            budget = form.Budget,
            favouriteArtist = form.FavouriteArtist,
            favouriteShow = form.FavouriteShow,
            personality = form.Personality,
            interests = (form.Interests ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList()
        };

        try
        {
            var res = await http.PostAsJsonAsync("api/recommendations", payload);
            if (!res.IsSuccessStatusCode)
                return (null, (await res.Content.ReadAsStringAsync()).Trim('"'));

            var matches = await res.Content.ReadFromJsonAsync<List<GiftMatch>>();
            return (matches, null);
        }
        catch (HttpRequestException)
        {
            return (null, "Could not reach the Giftee API. If it was asleep, try again in a minute.");
        }
    }
}