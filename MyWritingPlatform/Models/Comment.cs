using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyWritingPlatform.Models
{
    public class Comment
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Комментарий")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата публикации")]
        public DateTime Published { get; set; }

        public User User { get; set; }

        public Post Post { get; set; }
    }
}
