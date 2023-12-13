using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupSpace2022.Data;
using GroupSpace2022.Models;
using Microsoft.AspNetCore.Authorization;
using GroupSpace2022.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace GroupSpace2022.Controllers
{
    [Authorize (Roles = "SystemAdministrator")]
    public class ParametersController : GroupSpace2022Controller
    {

        public ParametersController(GroupSpace2022Context context,
                                    IHttpContextAccessor httpContextAccessor,
                                    ILogger<GroupSpace2022Controller> logger)
            : base(context, httpContextAccessor, logger)
        {
        }

        // GET: Parameters
        public async Task<IActionResult> Index()
        {
              return View(await _context.Parameters.OrderBy(p => p.Name).ToListAsync());
        }

 
        // GET: Parameters/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Parameters == null)
            {
                return NotFound();
            }

            var parameter = await _context.Parameters.FindAsync(id);
            if (parameter == null)
            {
                return NotFound();
            }
            return View(parameter);
        }

        // POST: Parameters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Value,Added,OutOfUse")] Parameter parameter)
        {
            if (id != parameter.Name)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parameter);
                    Globals.EditParameter(parameter.Name, parameter.Value, parameter.Destination);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParameterExists(parameter.Name))
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
            return View(parameter);
        }


        private bool ParameterExists(string id)
        {
          return (_context.Parameters?.Any(e => e.Name == id)).GetValueOrDefault();
        }
    }
}
