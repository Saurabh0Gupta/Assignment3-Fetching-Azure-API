using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3_version_1._0._0._0
{
  
    public class Project2
    {
        public string?  Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string CreatedDate { get; set; }
        public Dictionary<string, string> customFields { get; set; } = new Dictionary<string, string>();
        public List<TaskItem>? Tasks { get; set; } = new List<TaskItem>();
    }

    class DictonaryPro
    {
        public Dictionary<string, Project2>? ProjectJson { get; set; }= new Dictionary<string, Project2>();
    }

}


public class TaskItem
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public Assignee? Assignees { get; set; }
        public Reporter? Reporters { get; set; }
        public string? Priority { get; set; }
        public List<string>? Tags { get; set; }//it should be list array for the future
        public string StartDate { get; set; }
        //public DateTime DueDate { get; set; } // due date is not present in the azure api
        //public string TimeEstimate { get; set; }
        //public string TimeSpent { get; set; }
        //public string Resolution { get; set; }
        //public List<Subtask> Subtasks { get; set; } //not present in azure api 
        public List<Comment>? Comments { get; set; }
        //public List<Attachment> Attachments { get; set; }
        //public List<Timelog> Timelogs { get; set; }
        public Dictionary<string, string>? CustomFields_task { get; set; }=new Dictionary<string, string>();
    }

    public class Assignee
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }

    public class Reporter       
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }

    public class Subtask
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public List<Assignee> Assignees { get; set; }
    }

    public class Comment
    {
        public string? Id { get; set; }
        public Author? Authorr { get; set; }
        public string? Text { get; set; }
        public string? Timestamp { get; set; }
    public Comment(string id, string text, Author aut, string timestamp)
    {
        Id = id;
        Authorr = aut;
        Text = text;
        Timestamp = timestamp;
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

    public class User
    {
        public string? Id { get; set; }
        public string?   Name { get; set; }
        public string? Email { get; set; }
    }

    public class Resource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Permissions { get; set; }
    }




