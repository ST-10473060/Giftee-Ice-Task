using System.Text.Json;
using Giftee.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Giftee.Api.Data;

public static class DbSeeder
{
    // Retries because the database may still be starting (docker, or a sleeping Neon DB).
    public static async Task InitialiseAsync(GifteeDbContext db, int attempts = 8)
    {
        for (var i = 1; i <= attempts; i++)
        {
            try
            {
                await db.Database.EnsureCreatedAsync();
                if (await db.Gifts.AnyAsync()) return;

                var path = Path.Combine(AppContext.BaseDirectory, "Data", "gifts.json");
                var json = await File.ReadAllTextAsync(path);
                var gifts = JsonSerializer.Deserialize<List<Gift>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

                db.Gifts.AddRange(gifts);
                await db.SaveChangesAsync();
                return;
            }
            catch (Exception ex) when (i < attempts)
            {
                Console.WriteLine($"DB not ready (attempt {i}/{attempts}): {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }
    }
}