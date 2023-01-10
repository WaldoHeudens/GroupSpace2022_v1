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
using GroupSpace2022.APIModels;
using Microsoft.AspNetCore.Identity;

namespace GroupSpace2022.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly GroupSpace2022Context _context;
        private readonly SignInManager<GroupSpace2022User> _signInManager;

        public AccountController(GroupSpace2022Context context, SignInManager<GroupSpace2022User> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // Dit is een eenvoudige login-methode gebruikmakend van de bestaande Identity-structuur
        // Hiervoor hebben we nodig:  Een login-model (deze zit in APIModels)


        // POST: api/Groups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("/API/Login")]
        [Route("Login")]
        public async Task<ActionResult<Boolean>> Login([FromBody]LoginModel @login)
        {
            var result = await _signInManager.PasswordSignInAsync(@login.UserName, @login.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }

    }
}
