using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GroupSpace2022.Data;
using GroupSpace2022.Models;
using GroupSpace2022.Migrations;

namespace GroupSpace2022.Controllers
{
    public class LanguagesController : GroupSpace2022Controller
    {

        public LanguagesController(GroupSpace2022Context context, IHttpContextAccessor httpContextAccessor, ILogger<GroupSpace2022Controller> logger)
            : base(context, httpContextAccessor, logger)
        {
        }

        // GET: Languages
        public async Task<IActionResult> Index()
        {
              return View(await _context.Language.ToListAsync());
        }

        // GET: Languages/Details/5

        // GET: Languages/Create
        public IActionResult Create()
        {
            Language model = new Language { Cultures = "" };

            return View(model);
        }

        // POST: Languages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Cultures,IsShown")] Language language)
        {
            if (language.Cultures == null)
                language.Cultures = "";
            if (ModelState.IsValid)
            {
                _context.Add(language);
                await _context.SaveChangesAsync();
                Language.Initialize(_context);
                // zorg dat het systeem beschikt over een nieuwe lijst van gebruikte cultures
                try
                {
                    var localizationOptions = new RequestLocalizationOptions()
                        .AddSupportedCultures(Language.SupportedCultures)
                        .AddSupportedUICultures(Language.SupportedCultures)
                        .SetDefaultCulture("nl-BE");
                }
                catch
                {
                    _context.Remove(language);
                    await _context.SaveChangesAsync();
                    ViewData["ErrorMessage"] = "De code moet 2 kleine letters zijn, de culturelijst telkens 2 hoofdletters, gescheiden door een ';'";
                    return View(language);
                }
                return RedirectToAction(nameof(Index));
            }

            return View(language);
        }

        // GET: Languages/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Language == null)
            {
                return NotFound();
            }

            var language = await _context.Language.FindAsync(id);
            if (language.Cultures == null)
                language.Cultures = "";
            return View(language);
        }

        // POST: Languages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Cultures,IsShown")] Language language)
        {
            if (language.Cultures == null)
                language.Cultures = "";
            Language oldLanguage = _context.Language.FirstOrDefault(l => l.Id == language.Id);

            if (ModelState.IsValid)
            {
                 _context.Update(language);
                 await _context.SaveChangesAsync();

                Language.Initialize(_context);
                // zorg dat het systeem beschikt over een nieuwe lijst van gebruikte cultures
                try
                {
                    var localizationOptions = new RequestLocalizationOptions()
                        .AddSupportedCultures(Language.SupportedCultures)
                        .AddSupportedUICultures(Language.SupportedCultures)
                        .SetDefaultCulture("nl-BE");
                }
                catch
                {
                    
                    _context.Update(oldLanguage);
                    await _context.SaveChangesAsync();
                    ViewData["ErrorMessage"] = "De code moet 2 kleine letters zijn, de culturelijst telkens 2 hoofdletters, gescheiden door een ';'";
                    return View(language);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(language);
        }
    }
}
