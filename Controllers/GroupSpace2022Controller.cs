using GroupSpace2022.Services;
using GroupSpace2022.Areas.Identity.Data;
using GroupSpace2022.Data;
using Microsoft.AspNetCore.Mvc;

namespace GroupSpace2022.Controllers
{
    public class GroupSpace2022Controller : Controller
    {
        protected readonly GroupSpace2022Context _context;
        protected readonly ILogger<GroupSpace2022Controller> _logger;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly GroupSpace2022User _user;

        public GroupSpace2022Controller(GroupSpace2022Context context, IHttpContextAccessor httpContextAccessor, ILogger<GroupSpace2022Controller> logger)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            // Haal de gebruiker op van deze request-afhandeling
            _user = Globals.GetUser(httpContextAccessor.HttpContext.User.Identity.Name);
        }
    }
}
