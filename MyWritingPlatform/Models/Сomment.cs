using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWritingPlatform.Models
{
    public class Сomment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Published { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
