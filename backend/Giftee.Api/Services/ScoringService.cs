using Giftee.Api.Models;

namespace Giftee.Api.Services;

public static class ScoringService
{
    public static List<GiftMatch> Recommend(IEnumerable<Gift> gifts, RecommendRequest req, int take = 5)
    {
        var interests = req.Interests
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .Select(i => i.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();
        var personality = req.Personality?.Trim().ToLowerInvariant();
        var artist = string.IsNullOrWhiteSpace(req.FavouriteArtist) ? null : req.FavouriteArtist.Trim().ToLowerInvariant();
        var show = string.IsNullOrWhiteSpace(req.FavouriteShow) ? null : req.FavouriteShow.Trim().ToLowerInvariant();

        // Hard filters: age range and budget
        var eligible = gifts.Where(g =>
            (req.Age is null || (req.Age >= g.MinAge && req.Age <= g.MaxAge)) &&
            (req.Budget is null || g.MinPrice <= req.Budget)).ToList();

        var matches = new List<GiftMatch>();
        foreach (var g in eligible)
        {
            var tags = g.Tags.Select(t => t.ToLowerInvariant()).ToHashSet();
            var score = 0;
            var reasons = new List<string>();

            foreach (var i in interests)
            {
                if (tags.Contains(i) || g.Category.Equals(i, StringComparison.OrdinalIgnoreCase))
                {
                    score += 3;
                    reasons.Add($"Matches their interest in {i}");
                }
                else if (g.Description.Contains(i, StringComparison.OrdinalIgnoreCase))
                {
                    score += 1;
                    reasons.Add($"Related to {i}");
                }
            }

            if (artist != null)
            {
                if (Matches(g, tags, artist))
                {
                    score += 6;
                    reasons.Add($"Inspired by {req.FavouriteArtist!.Trim()}");
                }
                else if (tags.Contains("music"))
                {
                    score += 1;
                    reasons.Add("A music pick for a music lover");
                }
            }

            if (show != null)
            {
                if (Matches(g, tags, show))
                {
                    score += 6;
                    reasons.Add($"Inspired by {req.FavouriteShow!.Trim()}");
                }
                else if (tags.Contains("tv") || tags.Contains("film"))
                {
                    score += 1;
                    reasons.Add("A pick for a TV and film fan");
                }
            }

            if (!string.IsNullOrEmpty(personality) &&
                g.Personalities.Any(p => p.Equals(personality, StringComparison.OrdinalIgnoreCase)))
            {
                score += 2;
                reasons.Add($"Fits their {personality} side");
            }

            if (score > 0)
                matches.Add(new GiftMatch { Gift = g, Score = score, Reasons = reasons });
        }

        // Fallback so the user never sees an empty page
        if (matches.Count == 0)
        {
            matches = eligible.Take(take).Select(g => new GiftMatch
            {
                Gift = g,
                Score = 0,
                Reasons = ["A popular all-rounder that suits most people"]
            }).ToList();
        }

        return matches.OrderByDescending(m => m.Score).ThenBy(m => m.Gift.Name).Take(take).ToList();
    }

    // Does this gift relate to an artist, show or genre the user typed?
    private static bool Matches(Gift g, HashSet<string> tags, string term)
    {
        if (tags.Contains(term)) return true;
        if (term.Length < 4) return false;
        return g.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
            || tags.Any(t => t.Length >= 4 && t.Contains(term));
    }
}