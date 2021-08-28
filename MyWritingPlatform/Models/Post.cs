using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWritingPlatform.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public DateTime Published { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<Tag> Tags { get; set; }
        public  List<Сomment> Сomments { get; set; }


    }
}
