using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Tests.Mapping.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<BlogPost> Posts { get; set; } = new List<BlogPost>();
    }
}
