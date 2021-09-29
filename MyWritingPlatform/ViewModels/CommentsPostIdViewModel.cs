using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWritingPlatform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWritingPlatform.ViewModels
{
    public class CommentsPostIdViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int PostId { get; set; }
            
        public string UserUpId { get; set; }

        public string  Published { get; set; }
    }
}
