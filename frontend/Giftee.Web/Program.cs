using Giftee.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Render tells the app which port to use through the PORT env var
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port)) builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ApiClient>(c =>
{
    var baseUrl = builder.Configuration["API_BASE_URL"]
        ?? throw new InvalidOperationException("API_BASE_URL is not set.");
    c.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Gifts}/{action=Index}/{id?}");

app.Run();