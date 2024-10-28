using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskClasses;
using UserClass;

namespace Assignment3
{
    internal class ProjectClass
    {
            public string? id { get; set; } = "N/A";
            public string? name { get; set; } = "N/A";
            public string? description { get; set; } = "N/A";
            public string startDate { get; set; } = "N/A";

            public string endDate { get; set; } = "YYYY-MM-DD";
            public string status { get; set; } = "N/A";
            public string priority { get; set; } = "N/A";
            public User owner { get; set; } 

            public Dictionary<string, string> customFields { get; set; } = new Dictionary<string, string>();
            public List<TaskItem>? Tasks { get; set; } = new List<TaskItem>();
        public List<Resource> resources { get; set; } = new List<Resource>();
    }
}
