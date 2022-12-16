using GroupSpace2022.Data;
using Microsoft.AspNetCore.Mvc;
using GroupSpace2022.Models.ViewModels;
using GroupSpace2022.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using GroupSpace2022.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GroupSpace2022.Controllers
{
    [Authorize (Roles = "UserAdministrator")]
    public class UsersController : GroupSpace2022Controller
    {

        public UsersController(GroupSpace2022Context context, IHttpContextAccessor httpContextAccessor, ILogger<GroupSpace2022Controller> logger)
            : base(context, httpContextAccessor, logger)
        {
        }

        public IActionResult Index(string userName, string firstName, string lastName, string email, int? pageNumber)
        {
            List<UserViewModel> vmUsers = new List<UserViewModel>();
            List<GroupSpace2022User> users = _context.Users
                                                .Where( u => u.UserName != "Dummy"
                                                        && (u.UserName.Contains(userName) || string.IsNullOrEmpty(userName))
                                                        && (u.FirstName.Contains(firstName) || string.IsNullOrEmpty(firstName))
                                                        && (u.LastName.Contains(lastName) || string.IsNullOrEmpty(lastName))
                                                        && (u.Email.Contains(email) || string.IsNullOrEmpty(email)))
                                                .ToList();
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
            ViewData["userName"] = userName;
            ViewData["firstName"] = firstName;
            ViewData["lastName"] = lastName;
            ViewData["email"] = email;
            if (pageNumber == null) pageNumber = 1;
            Paginas<UserViewModel> model = new Paginas<UserViewModel>(vmUsers, vmUsers.Count, 1, 10);
            return View(model);
        }

        public IActionResult Undelete(string userName)
        {
            GroupSpace2022User user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            user.Deleted = DateTime.MaxValue;
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string userName)
        {
            GroupSpace2022User user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            user.Deleted = DateTime.Now;
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Roles(string userName)
        {

            GroupSpace2022User user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            RoleViewModel rvm = new RoleViewModel()
            {
                UserName = userName,
                Roles = (from userRole in _context.UserRoles
                        where userRole.UserId == user.Id
                        orderby userRole.RoleId
                        select userRole.RoleId).ToList()
            };

            ViewData["RoleIds"] = new MultiSelectList(_context.Roles.OrderBy(c => c.Name), "Id", "Name",rvm.Roles);
            return View(rvm);
        }

        [HttpPost]
        public IActionResult Roles([Bind("UserName,Roles")] RoleViewModel _model)
        {
            GroupSpace2022User user = _context.Users.FirstOrDefault(u => u.UserName == _model.UserName);
            List<IdentityUserRole<string>> userRoles = _context.UserRoles.Where(ur => ur.UserId == user.Id).ToList();
            foreach (IdentityUserRole<string> ur in userRoles)
            {
                _context.Remove(ur);
            }
            if (_model.Roles != null)
                foreach (string roleId in _model.Roles)
                    _context.UserRoles.Add(new IdentityUserRole<string>() { RoleId = roleId, UserId = user.Id });
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
