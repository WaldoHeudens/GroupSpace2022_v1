using GroupSpace2022.Data;
using GroupSpace2022.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GroupSpace2022.Controllers
{
    public class UserGroupsController : GroupSpace2022Controller
    {

        public UserGroupsController(GroupSpace2022Context context, 
                                   IHttpContextAccessor contextAccessor,
                                   ILogger<GroupSpace2022Controller> logger) 
            : base(context, contextAccessor, logger)
        {
        }

        // GET: UserGroups
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserGroup.Include(u => u.Group).Include(u => u.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroup
                .Include(u => u.Group)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userGroup == null)
            {
                return NotFound();
            }

            return View(userGroup);
        }

        // GET: UserGroups/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(_context.Group, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: UserGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GroupId,UserId,Added,Left,BecameHost,NoHostAnymore")] UserGroup userGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Group, "Id", "Id", userGroup.GroupId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userGroup.UserId);
            return View(userGroup);
        }

        // GET: UserGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroup.FindAsync(id);
            if (userGroup == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_context.Group, "Id", "Id", userGroup.GroupId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userGroup.UserId);
            return View(userGroup);
        }

        // POST: UserGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GroupId,UserId,Added,Left,BecameHost,NoHostAnymore")] UserGroup userGroup)
        {
            if (id != userGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserGroupExists(userGroup.Id))
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
            ViewData["GroupId"] = new SelectList(_context.Group, "Id", "Id", userGroup.GroupId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userGroup.UserId);
            return View(userGroup);
        }

        // GET: UserGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroup
                .Include(u => u.Group)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userGroup == null)
            {
                return NotFound();
            }

            return View(userGroup);
        }

        // POST: UserGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userGroup = await _context.UserGroup.FindAsync(id);
            _context.UserGroup.Remove(userGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserGroupExists(int id)
        {
            return _context.UserGroup.Any(e => e.Id == id);
        }
    }
}
