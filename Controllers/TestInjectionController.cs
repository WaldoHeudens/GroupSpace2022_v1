using GroupSpace2022.Areas.Identity.Data;
using GroupSpace2022.Services;
using Microsoft.AspNetCore.Mvc;

namespace GroupSpace2022.Controllers
{
    public class TestInjectionController : Controller
    {
        //private readonly IMyUser _myUser;
        private readonly GroupSpace2022User _myUser;

        public TestInjectionController (IMyUser user)
        {
            _myUser = user.AppUser();
        }

        public IActionResult Index()
        {
            GroupSpace2022User myUser = _myUser;
            return View();
        }
    }
}
