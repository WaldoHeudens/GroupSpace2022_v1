using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupSpace2022.Data;
using GroupSpace2022.Models;
using GroupSpace2022.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using GroupSpace2022.Services;

namespace GroupSpace2022.Controllers
{
    public class GroupsController : GroupSpace2022Controller
    {
        private readonly IEmailSender _emailSender;

        public GroupsController(    GroupSpace2022Context context, 
                                    IHttpContextAccessor httpContextAccessor, 
                                    ILogger<GroupSpace2022Controller> logger, 
                                    IEmailSender emailSender)
            : base(context, httpContextAccessor, logger)
        {
            _emailSender = emailSender; 
        }

        // GET: Groups
        public async Task<IActionResult> Index(string searchField = " ")
        {
            // Haal de groepen op van de huidige gebruiker en de groepen waarvan deze (de enige) host is
            List<int> hosts = new List<int>();
            List<Group> groups = new List<Group>();
            List<int> isOnlyHost = new List<int>();

            foreach (UserGroup ug in _user.Groups)
            {
                // haal de groepen op waarvan de gebruiker host is
                if ((String.IsNullOrEmpty(searchField) ? true : ug.Group.Name.Contains(searchField)) && ug.Left > DateTime.Now)
                    groups.Add(ug.Group);

                // selecteer de groepen waarvoor hij de enige host is
                if (ug.BecameHost < DateTime.Now && ug.NoHostAnymore > DateTime.Now)
                {
                    hosts.Add(ug.GroupId);
                    if ((from g in _context.UserGroup
                         where g.GroupId == ug.GroupId
                                 && g.BecameHost < DateTime.Now
                                 && g.NoHostAnymore > DateTime.Now
                         select g.UserId).Count() == 1)
                        isOnlyHost.Add(ug.GroupId);
                }
            }

            groups = groups.OrderBy(g => g.Name).ToList();

            ViewData["Hosts"] = hosts;
            ViewData["isOnlyHost"] = isOnlyHost;
            return View(groups);
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Maak een nieuw groupViewModel en haal alle gegevens daarvoor op

            GroupViewModel groupViewModel = new GroupViewModel();

            var @group = await _context.Group
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@group == null)
            {
                return NotFound();
            }

            groupViewModel.Name = group.Name;
            groupViewModel.Description = group.Description;
            groupViewModel.Id = group.Id;
            groupViewModel.Started = group.Started;

            // Haal de gebruiker op die de groep heeft aangemaakt
            if (_user.Id == group.StartedById)
                groupViewModel.StartedBy = _user.FirstName + " " + _user.LastName;
            else
            {
                GroupSpace2022User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == group.StartedById);
                groupViewModel.StartedBy = user.FirstName + " " + user.LastName;
            }

            // Haal de groepsleden op
            var members = from ug in _context.UserGroup
                          where ug.GroupId == @group.Id
                             && ug.Left > DateTime.Now
                          orderby ug.User.LastName + " " + ug.User.FirstName
                          select ug;

            groupViewModel.Members = await (from m in members
                                            select m.User.FirstName + " " + m.User.LastName)
                                     .ToListAsync();

            groupViewModel.Hosts = await (from m in members
                                          where m.BecameHost < DateTime.Now && m.NoHostAnymore > DateTime.Now
                                          select m.User.FirstName + " " + m.User.LastName)
                                    .ToListAsync();
            groupViewModel.isHost = (from m in members
                                     where (m.UserId == _user.Id
                                         && m.BecameHost < DateTime.Now
                                         && m.NoHostAnymore > DateTime.Now)
                                     select m.UserId).Count() > 0;
            return View(groupViewModel);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            // Geef een lege group mee waarin ik mijn default-values heb aangepast
            Group @group = new Group() { Started = DateTime.Now, Ended = DateTime.MaxValue };

            return View(@group);
        }


        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Started,Ended")] Group @group)
        {
            // Haal gebruiker op binnen de huidige instantie van GroupSpace2022Context
            GroupSpace2022User  user = _context.Users.FirstOrDefault(u => u.Id == _user.Id);

            // Bewaar de groep, maar ook de connectie met de gebruiker
            group.StartedById = _user.Id;
            group.EndedById = "dummy";
            _context.Add(@group);
            UserGroup userGroup = new UserGroup
            {
                Added = DateTime.Now,
                BecameHost = DateTime.Now,
                Group = group,
                Left = DateTime.MaxValue,
                NoHostAnymore = DateTime.MaxValue,
                User = user
            };
            _context.Add(userGroup);
            user.ActualGroup = group;
            _context.Update(user);
            await _context.SaveChangesAsync();

            // Pas de _user aan, zodat de groepen in het menu juist wordt weergegeven
            _user.ActualGroup = group;
            _user.Groups.Add(userGroup);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Group.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }
            return View(@group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Started,Ended")] Group @group)
        {
            if (id != @group.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Group
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Group group = _context.Group.Where(g => g.Id == id).FirstOrDefault();
            group.Ended = DateTime.Now;
            group.EndedById = _user.Id;
            _context.Update(group);
            List<UserGroup> usergroups = (from ug in _context.UserGroup
                                          where ug.GroupId == id && ug.Left > DateTime.Now
                                          select ug).ToList();
            foreach (UserGroup ug in usergroups)
            {
                ug.Left = DateTime.Now;
                _context.Update(ug);
            }
            if (_user.ActualGroupId == id)
            {
                GroupSpace2022User user = _context.Users.FirstOrDefault(u => u.Id == _user.Id);
                user.ActualGroupId = 1;
                foreach (UserGroup ug in _user.Groups)
                    if (ug.GroupId != id && ug.Left > DateTime.Now)
                    {
                        user.ActualGroupId = ug.GroupId;
                    }
                user.ActualGroup = _context.Group.FirstOrDefault(g => g.Id == user.ActualGroupId);
                _context.Update(user);
            }
            _context.SaveChanges();

            // Zorg ervoor dat de gebruiker geupdated wordt
            Globals.ReloadUser(_user.UserName, _context);
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
            return _context.Group.Any(e => e.Id == id);
        }

        public ActionResult Invite(int id = 1)
        {
            if (id == 1)
                id = _user.ActualGroupId.Value;
            ViewData["GroupName"] = _context.Group.FirstOrDefault(g => g.Id == id).Name;
            return View(new InviteViewModel { GroupId = id });
        }

        [HttpPost]
        public async Task<ActionResult> Invite([Bind("FirstName, Surname, Email, GroupId")] InviteViewModel invitation)
        {
            if (ModelState.IsValid)
            {
                Guid code = Guid.NewGuid();
                var callbackUrl = Url.Page(
                    "/Tokens/GroupInvitation",
                    pageHandler: null,
                    values: new { email = invitation.Email, code = code },
                    protocol: Request.Scheme);
                string groupName = (from g in _context.Group where g.Id == invitation.GroupId select g.Name).FirstOrDefault(); ;

                _context.Token.Add(new Models.Token
                {
                    Added = DateTime.Now,
                    AddedBy = _user.Id,
                    ConnectedId = invitation.GroupId,
                    Connection = invitation.Email
                });
                await _context.SaveChangesAsync();
                string userName = _user.FirstName + " " + _user.LastName;
                string mailBodyString = $"" + "Bevestig je lidmaatschap";
                mailBodyString = string.Format(mailBodyString, groupName, userName, invitation.Email);

                await _emailSender.SendEmailAsync(invitation.Email, "Bevestiging van een groeplidmaatschap op GroupSpace", mailBodyString);
                return RedirectToAction("Index", "Home");
            }
            return View(invitation);
        }


        public ActionResult Select(int id)
        {
            GroupSpace2022User  user = _context.Users.FirstOrDefault(u => u.Id == _user.Id);
            user.ActualGroup = _context.Group.FirstOrDefault(g => g.Id == id);
            user.ActualGroupId = id;
            _context.Update(user);
            _context.SaveChanges();
            _user.ActualGroup = user.ActualGroup;
            _user.ActualGroupId = id;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Hosts(int id)
        {
            return RedirectToAction("Index");
        }

        public ActionResult Members(int id, string searchField = "")
        {
            UserGroup thisUserGroup = _user.Groups.FirstOrDefault(g => g.GroupId == id);
            Group group = thisUserGroup.Group;
            List<UserGroup> userGroups = _context.UserGroup
                                        .Where(ug => ug.GroupId == id && ug.Left > DateTime.Now
                                                && (searchField == "" || (ug.User.FirstName + " " + ug.User.LastName).Contains(searchField)))
                                        .Include(ug => ug.User)
                                        .ToList();
            List<MemberViewModel> members = new List<MemberViewModel>();
            foreach (UserGroup ug in userGroups)
            {
                members.Add(new MemberViewModel
                {
                    Added = ug.Added,
                    isHost = ug.BecameHost < DateTime.Now && ug.NoHostAnymore > DateTime.Now,
                    Name = ug.User.FirstName + " " + ug.User.LastName,
                    UserId = ug.UserId
                });
            }
            ViewData["SearchField"] = searchField;
            ViewData["GroupName"] = group.Name;
            ViewData["GroupId"] = id.ToString();
            ViewData["isHost"] = thisUserGroup.BecameHost < DateTime.Now && thisUserGroup.NoHostAnymore > DateTime.Now;
            return View(members);
        }

        public async Task<IActionResult> DeleteMember(int? groupId, string? userId)
        {
            if (groupId == null || userId == null)
            {
                return NotFound();
            }

            var @userGroup = await _context.UserGroup
                            .Where(ug => ug.UserId == userId && ug.GroupId == groupId)
                            .Include(ug => ug.User)
                            .Include(ug => ug.Group)
                            .FirstOrDefaultAsync();
            if (@userGroup == null)
            {
                return NotFound();
            }

            return View(@userGroup);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("DeleteMember")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMemberConfirmed(int groupId, string userId)
        {
            UserGroup userGroup = _context.UserGroup
                                   .Where(ug => ug.UserId == userId && ug.GroupId == groupId)
                                   .FirstOrDefault();
            userGroup.Left = DateTime.Now;
            if (userGroup.BecameHost < DateTime.Now && userGroup.NoHostAnymore > DateTime.Now)
                userGroup.NoHostAnymore = DateTime.Now;
            _context.Update(userGroup);
            ViewData["user"] = _user; 
            return RedirectToAction("Members");
        }
    }
}
