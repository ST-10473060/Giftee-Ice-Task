namespace Giftee.Api.Models;

public class Gift
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string[] Tags { get; set; } = [];
    public string[] Personalities { get; set; } = [];
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public string Description { get; set; } = "";
}