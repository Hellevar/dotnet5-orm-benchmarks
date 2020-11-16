using System;
using System.Collections.Generic;

namespace Runner.Models
{
    public class Blog
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public List<Post> Posts { get; set; }
    }
}
