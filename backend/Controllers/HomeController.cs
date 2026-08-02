using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

// Serves the single-page app shell. The built React artifact (produced by
// `vite build` into wwwroot) is referenced from the Razor view via the Vite
// manifest. Client-side routing is hash-based, so every non-API path renders
// this same shell.
public class HomeController : Controller
{
    public IActionResult Index() => View();
}
