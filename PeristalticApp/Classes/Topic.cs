using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeristalticApp.Classes
{
    public class Topic
    {
        public string Name { get; set; }
        public bool Active { get; set; }

        public Topic()
        {
            Name = "";
            Active = true;
        }
        public Topic(string name)
        {
            Name = name;
            Active = true;
        }
    }
}
