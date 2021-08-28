using Microsoft.AspNetCore.Identity;
using System;

namespace MyWritingPlatform.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
        public string Login { get; set; }

    }
}
