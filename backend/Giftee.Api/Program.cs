using Giftee.Api.Data;
using Giftee.Api.Models;
using Giftee.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Render tells the app which port to use through the PORT env var
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port)) builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var conn = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default is not set.");

builder.Services.AddDbContext<GifteeDbContext>(o => o.UseNpgsql(conn));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbSeeder.InitialiseAsync(scope.ServiceProvider.GetRequiredService<GifteeDbContext>());
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }))
   .WithTags("Health");

app.MapGet("/api/gifts", async (GifteeDbContext db) =>
    Results.Ok(await db.Gifts.AsNoTracking().OrderBy(g => g.Name).ToListAsync()))
   .WithTags("Gifts");

app.MapPost("/api/recommendations", async (RecommendRequest req, GifteeDbContext db) =>
{
    if (req.Age is < 1 or > 120) return Results.BadRequest("Age must be between 1 and 120.");
    if (req.Budget is < 0) return Results.BadRequest("Budget cannot be negative.");

    var hasInput = req.Interests.Any(i => !string.IsNullOrWhiteSpace(i))
        || !string.IsNullOrWhiteSpace(req.FavouriteArtist)
        || !string.IsNullOrWhiteSpace(req.FavouriteShow)
        || !string.IsNullOrWhiteSpace(req.Personality);
    if (!hasInput)
        return Results.BadRequest("Give at least one interest, artist, show or personality type.");

    var gifts = await db.Gifts.AsNoTracking().ToListAsync();
    return Results.Ok(ScoringService.Recommend(gifts, req));
})
.WithTags("Recommendations");

app.Run();