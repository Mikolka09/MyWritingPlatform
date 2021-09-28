﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWritingPlatform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWritingPlatform.ViewModels
{
    public class PostsGetIdViewModel
    {
  
        public int Id { get; set; }

        public string UserUpId { get; set; }

        public string ImgPost { get; set; }

        public string Title { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public string Published { get; set; }

        public bool Censor { get; set; }

        public string UserName { get; set; }

        public List<CommentViewModel> Comments { get; set; }

    }
}
