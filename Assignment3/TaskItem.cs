using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment3;
using UserClass;

namespace TaskClasses
{
    public class TaskItem
    {
        public string? id { get; set; } = "N/A";
        public string? title { get; set; } = "N/A";
        public string? description { get; set; } = "N/A";
        public string? type { get; set; } = "N/A";
        public string? status { get; set; } = "N/A";
        public User? assignees { get; set; } = new User { email = "N/A", id = "N/A", name = "N/A" };//should be array
        public User? reporters { get; set; } = new User { email = "N/A", id = "N/A", name = "N/A" };
        public string? priority { get; set; } = "N/A";
        public List<string>? tags { get; set; }//it should be list array for the future
        public string? startDate { get; set; } = "YYYY-MM-DD";
        public string? dueDate { get; set; } = "YYYY-MM-DD";// due date is not present in the azure api
        public string? timeEstimate { get; set; } = "N/A";
        public string? timeSpent { get; set; } = "N/A";
        public string? resolution { get; set; } = "N/A"; 
        public List<Subtask>? subtasks { get; set; } //not present in azure api 
        public List<Comment>? comments { get; set; }
        public List<Attachment>? attachments { get; set; }
        public List<Timelog>? timelogs { get; set; }
        public Dictionary<string, string>? customFields { get; set; } = new Dictionary<string, string>();
        
    }

    public class Assignee
    {
        public string? id { get; set; } = "N/A";
        public string? name { get; set; } = "N/A";
        public string? email { get; set; } = "N/A";
    }

    public class Reporter
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
    }

    public class Subtask
    {
        public string? id { get; set; } = "N/A";
        public string? title { get; set; } = "N/A";
        public string? status { get; set; } = "N/A";
        public List<Assignee>? Assignees { get; set; }
    }

    public class Comment
    {
        public string? id { get; set; }
        public Author? Authorr { get; set; }
        public string? text { get; set; }
        public string? timestamp { get; set; }
        public Comment(string id, string text, Author aut, string timestamp)
        {
            this.id = id;
            this.Authorr = aut;
            this.text = text;
            this.timestamp = timestamp;
        }
    }

    public class Author
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public Author(string Id, string Name, string email)
        {
            this.Id = Id;
            this.Name = Name;
            this.Email = email;
        }

    }

    public class Attachment
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public double FileSize { get; set; }  // File size in KB
        public DateTime UploadDate { get; set; }
        public string Url { get; set; }
    }

    public class Timelog
    {
        public string Id { get; set; }
        public User User { get; set; }
        public string TimeSpent { get; set; }
        public DateTime DateLogged { get; set; }
    }

    

    public class Resource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Permissions { get; set; }
    }
    class DictonaryPro
    {
        public Dictionary<string, ProjectClass>? ProjectJson { get; set; }= new Dictionary<string, ProjectClass>();
    }

}







