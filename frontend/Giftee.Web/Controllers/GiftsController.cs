using Giftee.Web.Models;
using Giftee.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Giftee.Web.Controllers;

public class GiftsController(ApiClient api) : Controller
{
    [HttpGet]
    public IActionResult Index() => View(new GiftForm());

    [HttpPost]
    public async Task<IActionResult> Index(GiftForm form)
    {
        var (matches, error) = await api.RecommendAsync(form);
        if (error != null) ViewBag.Error = error;
        else ViewBag.Matches = matches;
        return View(form);
    }
}