// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using GroupSpace2022.Areas.Identity.Data;
using GroupSpace2022.Models;
using GroupSpace2022.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GroupSpace2022.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<GroupSpace2022User> _userManager;
        private readonly SignInManager<GroupSpace2022User> _signInManager;

        public IndexModel(
            UserManager<GroupSpace2022User> userManager,
            SignInManager<GroupSpace2022User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Voornaam")]
            public string FirstName { get; set; }
            [Display(Name = "Achternaam")]
            public string LastName { get; set; }
            [Phone]
            [Display(Name = "Telefonnummer")]
            public string PhoneNumber { get; set; }
            [Display(Name = "Taal")]
            public string LanguageId { get; set; }
        }

        private async Task LoadAsync(GroupSpace2022User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                LanguageId = user.LanguageId
            };
            ViewData["Languages"] = Language.Languages.Where(l => l.IsShown).ToList();
            ViewData["LanguageId"] = user.LanguageId;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            bool haveToUpdate = false;
            if (Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
                haveToUpdate = true;
            }
            if (Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
                haveToUpdate = true;
            }
            if (Input.LanguageId != user.LanguageId)
            {
                user.LanguageId = Input.LanguageId;
                haveToUpdate = true;

                // verwijder de gebruiker uit de actieve SessionUser-lijst
                GroupSpace2022User gUser = Globals.GetUser(user.UserName);
                gUser.LanguageId = Input.LanguageId;
                gUser.Language = Language.LanguagesDictionary[Input.LanguageId];

                // Update the language/culture
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(Input.LanguageId)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            }
            if (haveToUpdate)
                await _userManager.UpdateAsync(user);
            
            // zorg dat de actieve gebruiker geupdated wordt
            // Globals.ReloadUser(user.UserName, dbContext)

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
