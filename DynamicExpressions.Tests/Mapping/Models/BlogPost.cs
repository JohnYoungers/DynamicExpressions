using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Tests.Mapping.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }

        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
