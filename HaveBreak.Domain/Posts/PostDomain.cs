using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HaveBreak.Domain.Posts
{
    public class PostDomain
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public List<string> Comments { get; set; }

        public int Likes { get; set; }
    }
}
