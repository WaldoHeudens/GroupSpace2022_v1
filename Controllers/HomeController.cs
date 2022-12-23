using GroupSpace2022.Data;
using GroupSpace2022.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GroupSpace2022.Controllers
{
    public class HomeController : GroupSpace2022Controller
    {

        public HomeController(GroupSpace2022Context context, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<GroupSpace2022Controller> logger)
            : base(context, httpContextAccessor, logger)
        {
        }

        public IActionResult Index(string searchField)
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}