using Microsoft.AspNetCore.Identity;
using System;

namespace MyWritingPlatform.Models
{
    public class User : IdentityUser
    {
        public string Avatar { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Login { get; set; }

        public int Year { get; set; }     

        public DateTime DateTimeRegister { get; set; }

    }
}
