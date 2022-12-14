using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GroupSpace2022.Data;
using GroupSpace2022.Models;

namespace GroupSpace2022.Controllers
{
    public class MediaController : GroupSpace2022Controller
    {

        public MediaController(GroupSpace2022Context context, IHttpContextAccessor httpContextAccessor, ILogger<GroupSpace2022Controller> logger)
            : base(context, httpContextAccessor, logger)
        {
        }

        // GET: Media
        public async Task<IActionResult> Index()
        {
              return View(await _context.Media.Where(m => m.Deleted > DateTime.Now).Include(m => m.Categories).ToListAsync());
        }

        // GET: Media/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Media == null)
            {
                return NotFound();
            }

            var media = await _context.Media
                .FirstOrDefaultAsync(m => m.Id == id);
            if (media == null)
            {
                return NotFound();
            }

            return View(media);
        }

        // GET: Media/Create
        public IActionResult Create()
        {
            ViewData["CategoryIds"] = new MultiSelectList(_context.Category.OrderBy(c => c.Name), "Id", "Name");
            Media media = new Media();
            return View(media);
        }

        // POST: Media/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Added, CategoryIds")] Media media)
        {
            if (ModelState.IsValid)
            {
                if (media.Categories == null)
                    media.Categories = new List<Category>();
                foreach (int id in media.CategoryIds)
                {
                    media.Categories.Add(_context.Category.FirstOrDefault(c => c.Id == id));
                }
                _context.Add(media);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryIds"] = new MultiSelectList(_context.Category.OrderBy(c => c.Name), "Id", "Name", media.CategoryIds);
            return View(media);
        }

        // GET: Media/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Media == null)
            {
                return NotFound();
            }

            var media = await _context.Media.Include(m => m.Categories).FirstAsync(m => m.Id==id);
            //media.Categories = _context.Category.Include(c => c.Medias).Where(m => m.Medias.Contains(media)).ToList();
            media.CategoryIds = new List<int>();
            foreach(Category c in media.Categories)
            {
                media.CategoryIds.Add(c.Id);
            }

            ViewData["CategoryIds"] = new MultiSelectList(_context.Category.OrderBy(c => c.Name), "Id", "Name", media.CategoryIds); 
            if (media == null)
            {
                return NotFound();
            }
            return View(media);
        }

        // POST: Media/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Added, CategoryIds")] Media media)
        {
            if (id != media.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Media existingMedia = _context.Media.Include(m => m.Categories).First(m => m.Id==id);
                    existingMedia.Name = media.Name;
                    existingMedia.Description = media.Description;
                    existingMedia.Categories.Clear();

                    foreach (int catId in media.CategoryIds)
                    {
                        existingMedia.Categories.Add(_context.Category.FirstOrDefault(c => c.Id == catId));
                    }
                    _context.Update(existingMedia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MediaExists(media.Id))
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
            ViewData["CategoryIds"] = new MultiSelectList(_context.Category.OrderBy(c => c.Name), "Id", "Name", media.CategoryIds);
            return View(media);
        }

        // GET: Media/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Media == null)
            {
                return NotFound();
            }

            var media = await _context.Media
                .FirstOrDefaultAsync(m => m.Id == id);
            if (media == null)
            {
                return NotFound();
            }

            return View(media);
        }

        // POST: Media/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Media == null)
            {
                return Problem("Entity set 'GroupSpace2022Context.Media'  is null.");
            }
            var media = await _context.Media.FindAsync(id);
            media.Deleted = DateTime.Now;
            if (media != null)
            {
                _context.Media.Update(media);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MediaExists(int id)
        {
          return _context.Media.Any(e => e.Id == id);
        }
    }
}
