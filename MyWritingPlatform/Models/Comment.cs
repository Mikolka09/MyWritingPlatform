using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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

        public string UserId { get; set; }
        public User User { get; set; }
        public int? PostId { get; set; }
        public Post Post { get; set; }
    }
}
