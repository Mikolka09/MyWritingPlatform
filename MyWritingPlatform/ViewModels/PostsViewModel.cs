using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWritingPlatform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWritingPlatform.ViewModels
{
    public class PostsViewModel
    {
  
        public int Id { get; set; }


        public string ImgPost { get; set; }

        public string Title { get; set; }


        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public string Published { get; set; }

        public bool Censor { get; set; }

        public string UserName { get; set; }

        public int? CategoryId { get; set; }

        public List<Category> Categories { get; set; }

        public int ComCount { get; set; }

        public List<Tag> Tags { get; set; }
        

    }
}
