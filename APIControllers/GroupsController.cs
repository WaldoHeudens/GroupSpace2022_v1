using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupSpace2022.Data;
using GroupSpace2022.Models;
using GroupSpace2022.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace GroupSpace2022.APIControllers
{
    [Route("api/[controller]")]
    [Authorize]                 // We kunnen deze alleen aanspreken als we aangemeld zijn
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly GroupSpace2022Context _context;

        public GroupsController(GroupSpace2022Context context)
        {
            _context = context;
        }

        // GET: api/Groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroup()
        {
            return await _context.Group.ToListAsync();
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(int id)
        {
            Group @group = await _context.Group.FindAsync(id);

            if (@group == null)
            {
                return NotFound();
            }

            return @group;
        }

        [HttpGet()]
        [Route("UserGroups/{userName}")]
        [Route("/UserGroups/{userName}")]
        public  List<Group> GetUserGroups(string? userName)
        {
            if (userName == null)
                userName = User.Identity.Name;

            GroupSpace2022User _user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (_user == null)
                return null;
            List < UserGroup > usergroups = _context.UserGroup.Where(u => u.UserId == _user.Id).ToList();
            List<Group> groups = new List<Group>();
            foreach (UserGroup ug in usergroups)
            {
                Group temp = _context.Group.FirstOrDefault(u => u.Id == ug.GroupId);
                temp.UserGroups = null;
                groups.Add(temp);
            }
            return groups;
        }

        // PUT: api/Groups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroup(int id, Group @group)
        {
            if (id != @group.Id)
            {
                return BadRequest();
            }

            _context.Entry(@group).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Groups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group @group)
        {
            _context.Group.Add(@group);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroup", new { id = @group.Id }, @group);
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var @group = await _context.Group.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }

            _context.Group.Remove(@group);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GroupExists(int id)
        {
            return _context.Group.Any(e => e.Id == id);
        }
    }
}
