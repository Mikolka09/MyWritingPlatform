using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWritingPlatform.Models
{
    public class Post
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Display(Name = "Картинка")]
        public string ImgPost { get; set; }

        [Required]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Краткое описание")]
        public string ShortDescription { get; set; }

        [Required]
        [Display(Name = "Текст статьи")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата публикации")]
        public DateTime Published { get; set; }

        [Required]
        [Display(Name = "Цензура")]
        public bool Censor { get; set; }

        [Display(Name = "Категория")]
        public string UserId { get; set; }
        public User User { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Comment> Сomments { get; set; }


    }
}
