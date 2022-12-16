#nullable disable
using Microsoft.AspNetCore.Mvc;
using GroupSpace2022.Models;
using Microsoft.AspNetCore.Authorization;
using GroupSpace2022.Controllers;
using GroupSpace2022.Data;
using GroupSpace2022.Areas.Identity.Data;
using GroupSpace2022.Services;

namespace GroupSpace.Controllers
{
    public class TokensController : GroupSpace2022Controller
    {

        public TokensController(GroupSpace2022Context context,
                                IHttpContextAccessor httpContextAccessor,
                                ILogger<GroupSpace2022Controller> logger)
            :base(context, httpContextAccessor, logger)
        {
        }

        [Authorize]
        public async Task<IActionResult> GroupInvitation(string email, Guid code)
        {
            GroupSpace2022User user = _context.Users.FirstOrDefault(u => u.Id == _user.Id);
            Token token = _context.Token.FirstOrDefault(t => t.Id == code);
            Group group = _context.Group.FirstOrDefault(g => g.Id == token.ConnectedId);
            UserGroup userGroup = new UserGroup
            {
                Added = DateTime.Now,
                BecameHost = DateTime.MaxValue,
                Group = group,
                GroupId = group.Id,
                Left = DateTime.MaxValue,
                NoHostAnymore = DateTime.MaxValue,
                User = user,
                UserId = user.Id
            };
            user.ActualGroup = group;
            user.ActualGroupId = group.Id;
            _context.Update(User);
            _context.Add(userGroup);
            await _context.SaveChangesAsync();

            // Zorg dat de gebruiker geupdated wordt (groeplijst moet aangepast worden)
            Globals.ReloadUser(_user.UserName, _context);
            return RedirectToAction("Index", "Home");
        }
    }
}
