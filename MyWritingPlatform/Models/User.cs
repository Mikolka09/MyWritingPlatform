﻿using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyWritingPlatform.Models
{
    public class User : IdentityUser
    {
        [Display(Name = "Аватар")]
        public string Avatar { get; set; }

        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "Год рождения")]
        public int Year { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата регистрации")]
        public DateTime DateTimeRegister { get; set; }

    }
}
