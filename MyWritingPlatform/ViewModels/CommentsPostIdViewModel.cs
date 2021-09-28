using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWritingPlatform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWritingPlatform.ViewModels
{
    public class CommentsPostIdViewModel: Comment
    {    
        public int PostId { get; set; }
            
        public string UserUpId { get; set; }

    }
}
