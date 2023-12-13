using GroupSpace2022.Areas.Identity.Data;
using GroupSpace2022.Data;
using GroupSpace2022.Models;
using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NETCore.MailKit.Infrastructure.Internal;
using SQLitePCL;

namespace GroupSpace2022.Services
{
    public class Globals
    {
        // Global systemparameters
        static Dictionary<string, string> Parameters { get; set; }
        static public WebApplication App { get; set; }



        // User management and userstatistics
        static Timer CleanUpTimer;
        struct UserStatistics
        {
            public DateTime FirstEntered { get; set; }
            public DateTime LastEntered { get; set; }
            public int Count { get; set; }
            public GroupSpace2022User User { get; set; }
        }


        readonly RequestDelegate _next; // verwijzing naar volgende middelware methode
        static Dictionary<string, UserStatistics> UserDictionary = new Dictionary<string, UserStatistics>(); 


        // Middleware constructor
        public Globals(RequestDelegate next)
        {
            _next = next;
            CleanUpTimer = new Timer(CleanUp, null, 21600000, 21600000);
        }


        // Middleware task
        public async Task Invoke(HttpContext httpContext, GroupSpace2022Context dbContext)
        {
            // Haal de gebruikersnaam op
            string name = httpContext.User.Identity.Name == null ? "dummy" : httpContext.User.Identity.Name;
            
            
            try  // Als de gebruiker al eerder opgehaald werd
            {
                UserStatistics us = UserDictionary[name];
                us.Count++;
                us.LastEntered = DateTime.Now;
            }
            catch // Als die nog niet opgehaald werd: Doe dat dan nu
            {
                AddUser(name, dbContext);
            }

            // Voer de volgende middleware task uit
            await _next(httpContext);
        }

        private static void AddUser(string name, GroupSpace2022Context dbContext, int count = 1)
        {
            GroupSpace2022User user = dbContext.Users
            .Include(u => u.ActualGroup)
            .ThenInclude(ag => ag.UserGroups)
            .ThenInclude(ug => ug.User)
            .FirstOrDefault(u => u.UserName == name);

            // Haal ook de groepen op van deze gebruiker
            user.Groups = dbContext.UserGroup
                            .Where(ug => ug.UserId == user.Id && ug.Left > DateTime.Now && ug.Group.Ended > DateTime.Now)
                            .Include(ug => ug.Group)
                            .Include(ug => ug.User)
                            .ToList();

            // Voeg de gebruiker aan de dictionary toe
            UserDictionary[name] = new UserStatistics
            {
                User = user,
                Count = 1,
                FirstEntered = DateTime.Now,
                LastEntered = DateTime.Now
            };
        }

        public static GroupSpace2022User GetUser(string? userName)
        {
            try
            {
                return UserDictionary[userName == null ? "dummy" : userName].User;
            }
            catch
            {
                return null;
            }
        }


        // Voer een periodieke cleanup uit
        private void CleanUp(object? state)
        {
            DateTime checkTime = DateTime.Now - new TimeSpan(0, 6, 0, 0, 0);
            Dictionary<string, UserStatistics> remove = new Dictionary<string, UserStatistics>();
            foreach (KeyValuePair<string, UserStatistics> us in UserDictionary)
            {
                if (us.Value.LastEntered < checkTime)
                    remove[us.Key] = us.Value;
            }
            foreach (KeyValuePair<string, UserStatistics> us in remove)
                UserDictionary.Remove(us.Key);
        }

        public static void ReloadUser(string userName, GroupSpace2022Context dbContext)
        {
            int count = UserDictionary[userName].Count;
            UserDictionary.Remove(userName);
            AddUser(userName, dbContext, count);
        }

        public static void InitializeParameters(GroupSpace2022Context context)
        {
            Parameters = new Dictionary<string, string>();
            List<Parameter> parameters = context.Parameters.ToList();
            foreach(Parameter p in parameters)
            {
                Parameters[p.Name] = p.Value;
            }
            ConfigureEmail();

        }

        public static string GetParameter(string name)
        {
            return Parameters[name];
        }

        public static void EditParameter(string name, string value, string destination)
        {
            Parameters[name] = value;

            if (destination == "Mail")
                ConfigureEmail();
        }

        static void ConfigureEmail()
        {

            MailKitEmailSender mailsender = (MailKitEmailSender) App.Services.GetService<IEmailSender>();
            var options = mailsender.Options;
            options.Server = Parameters["Mail.Server"];
            options.Port = Convert.ToInt16(Parameters["Mail.Port"]);
            options.Account = Parameters["Mail.Account"];
            options.Password = Parameters["Mail.Password"];
            options.SenderEmail = Parameters["Mail.SenderEmail"];
            options.SenderName = Parameters["Mail.SenderName"];
            options.Security = Convert.ToBoolean(Parameters["Mail.Security"]);
        }
    }
}
