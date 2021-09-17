using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyWritingPlatform.ViewModels
{
    public class ChangePasswordViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Пароль должен быть не менее 6 символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый Пароль")]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Пароль должен быть не менее 6 символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Старый Пароль")]
        public string OldPassword { get; set; }
    }
}
