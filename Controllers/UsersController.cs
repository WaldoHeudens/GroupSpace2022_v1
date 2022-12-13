using GroupSpace2022.Data;
using Microsoft.AspNetCore.Mvc;
using GroupSpace2022.Models.ViewModels;
using GroupSpace2022.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace GroupSpace2022.Controllers
{
    [Authorize (Roles = "Beheerder")]
    public class UsersController : Controller
    {
        private readonly GroupSpace2022Context _context;

        public UsersController(GroupSpace2022Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<UserViewModel> vmUsers = new List<UserViewModel>();
            List<GroupSpace2022User> users = _context.Users.Where(u => u.UserName != "Dummy").ToList();
            foreach (GroupSpace2022User user in users)
            {
                vmUsers.Add(new UserViewModel
                {
                    Deleted = user.Deleted < DateTime.Now,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Roles = (from userRole in _context.UserRoles
                             where userRole.UserId == user.Id
                             orderby userRole.RoleId
                             select userRole.RoleId).ToList()
                });
            }
            return View(vmUsers);
        }
    }
}
