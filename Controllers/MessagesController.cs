using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GroupSpace2022.Data;
using GroupSpace2022.Models;
using Microsoft.AspNetCore.Authorization;

namespace GroupSpace2022.Controllers
{
    [Authorize]
    public class MessagesController : GroupSpace2022Controller
    {

        public MessagesController(GroupSpace2022Context context,
                                    IHttpContextAccessor httpContextAccessor,
                                    ILogger<GroupSpace2022Controller> logger)
            : base(context, httpContextAccessor, logger)
        {
        }

        // GET: Messages
        public async Task<IActionResult> Index(string titleFilter, string messageFilter, string contentMessage, string orderBy, int? pageNumber, string selectedMode = "R")
        {
            MessageIndexViewModel viewModel = new MessageIndexViewModel();
            List<MessageViewModel> messages = new List<MessageViewModel>();

            // Lijst alle messages op die we ontvangen hebben
            if (selectedMode == "R")
            {
                List<MessageDestination> messageDestinations = await _context.MessageDestinations
                                       .Include(md => md.Message)
                                       .Where(md => md.ReceiverId == _user.Id
                                                    && md.Deleted > DateTime.Now
                                                    && (string.IsNullOrEmpty(titleFilter) || md.Message.Title.Contains(titleFilter))
                                                    && (string.IsNullOrEmpty(messageFilter) || md.Message.Title.Contains(messageFilter)))
                                       .OrderBy(md => md.Received)
                                       .ToListAsync();
                foreach (MessageDestination md in messageDestinations)
                {
                    MessageViewModel mvm = new MessageViewModel
                    {
                        Attachments = new List<Media>(),
                        Content = md.Message.Content.Substring(0, Math.Min(20, md.Message.Content.Length)),
                        Destinies = null,
                        Groups = null,
                        MessageId = md.Message.Id,
                        Sent = md.Received,
                        Title = md.Message.Title
                    };
                    messages.Add(mvm);
                }
            }
            else
            {
                List<Message> tempMessages = await _context.Message
                                            .Where(m => m.SenderId == _user.Id
                                                        && (string.IsNullOrEmpty(titleFilter) || m.Title.Contains(titleFilter))
                                                        && (string.IsNullOrEmpty(messageFilter) || m.Title.Contains(messageFilter)))
                                            .Include(m => m.Destinations)
                                            .ThenInclude(m => m.Receiver)
                                            .ToListAsync();
                foreach (Message m in tempMessages)
                {
                    MessageViewModel mvm = new MessageViewModel
                    {
                        Attachments = new List<Media>(),
                        Content = m.Content.Substring(0, Math.Min(20, m.Content.Length)),
                        Destinies = (from d in m.Destinations
                                     orderby d.Receiver.LastName + " " + d.Receiver.FirstName
                                     select d.Receiver)
                                    .ToList(),
                        MessageId = m.Id,
                        Sent = m.Created,
                        Title = m.Title
                    };
                    messages.Add(mvm);
                }
            }

            ViewData["TitleField"] = string.IsNullOrEmpty(orderBy) ? "Sent" : "";
            ViewData["ContentField"] = orderBy == "Content" ? "Content_Desc" : "Content";
            ViewData["SentField"] = orderBy == "Title" ? "Title_Desc" : "Title";
            ViewData["OrderBy"] = orderBy;

            switch (orderBy)
            {
                case "Title":
                    messages = messages.OrderBy(m => m.Title).ToList();
                    break;
                case "Title_Desc":
                    messages = messages.OrderByDescending(m => m.Title).ToList();
                    break;
                case "Content":
                    messages = messages.OrderBy(m => m.Content).ToList();
                    break;
                case "Content_Desc":
                    messages = messages.OrderByDescending(m => m.Content).ToList();
                    break;
                case "Sent":
                    messages = messages.OrderBy(m => m.Sent).ToList();
                    break;
                default:
                    messages = messages.OrderByDescending(m => m.Sent).ToList();
                    break;
            }

            //// Lijst van groepen 
            //IQueryable<Group> groupsToSelect = from g in _context.Group orderby g.Name select g;

            //// Selectieveldje voor de mode van gebruik
            var modeItems = new List<SelectListItem>
            {
                new SelectListItem{Value="R", Text="Ontvangen", Selected = selectedMode == "R" },
                new SelectListItem{Value="S", Text="Verzonden", Selected = selectedMode == "S"}
            };

            viewModel.Messages = new Paginas<MessageViewModel>(messages, messages.Count, 1, 10);
            viewModel.ModesToSelect = new SelectList(modeItems, "Value", "Text");
            viewModel.SelectedMode = selectedMode;
            viewModel.TitleFilter = titleFilter;
            viewModel.MessageFilter = messageFilter;
            return View(viewModel);
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                                .Include(m => m.Destinations)
                                    .ThenInclude(d => d.Receiver)
                                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            try
            {
                MessageDestination md = message.Destinations.FirstOrDefault(d => d.ReceiverId == _user.Id);
                if (md.Read > DateTime.Now)
                {
                    md.Read = DateTime.Now;
                    _context.Update(md);
                    _context.SaveChangesAsync();
                }
            }
            catch { }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            MessageViewModel model = new MessageViewModel
            {
                DestinyIds = new List<string>(),
                GroupIds = new List<int>(),
            };
            ViewData["GroupIds"] = new MultiSelectList(_user.Groups, "GroupId", "Group.Name");
            ViewData["DestinyIds"] = new MultiSelectList(_user.ActualGroup.UserGroups, "User.Id", "User.FirstName");
            return View(model);
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,Attachments,GroupIds, DestinyIds")] MessageViewModel model)
        {
            if (ModelState.IsValid && (model.GroupIds != null || model.DestinyIds != null))
            {
                if (model.DestinyIds == null)
                    model.DestinyIds = new List<string>();

                foreach (int id in model.GroupIds)
                {
                    List<string> ids = (from g in _user.Groups
                                        where g.GroupId == id
                                        select g.UserId)
                                       .ToList();
                    model.DestinyIds.AddRange(ids);
                }
                model.DestinyIds = model.DestinyIds.Distinct().ToList();
                model.DestinyIds.Remove(_user.Id);

                List<MessageDestination> destinations = new List<MessageDestination>();

                // Media
                Message message = new Message
                {
                    Sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == _user.Id),
                    Content = model.Content,
                    Created = DateTime.Now,
                    Title = model.Title,
                    Destinations = destinations
                };
                foreach (string id in model.DestinyIds)
                {
                    destinations.Add(new MessageDestination
                    {
                        Deleted = DateTime.MaxValue,
                        Message = message,
                        Read = DateTime.MaxValue,
                        Received = DateTime.Now,
                        ReceiverId = id
                    });
                }
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupIds"] = new MultiSelectList(_user.Groups, "GroupId", "Group.Name");
            ViewData["DestinyIds"] = new MultiSelectList(_user.ActualGroup.UserGroups, "User.Id", "User.FirstName");
            return View(model);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var messageDestination = await _context.MessageDestinations
                                    .FirstOrDefaultAsync(md => md.MessageId == id && md.ReceiverId == _user.Id);
            messageDestination.Deleted = DateTime.Now;
            _context.Update(messageDestination);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Message.Any(e => e.Id == id);
        }
    }
}
