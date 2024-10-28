using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TaskClasses;
using Newtonsoft.Json;
using static Program;

namespace Fetching
{
   
    public class Project
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string CreatedDate { get; set; }
            public Dictionary<string, string>? CustomFields{ get; set;}=new Dictionary<string, string>();
            public List<WorkItem>? TaskArray { get; set; }= new List<WorkItem>();
            public Project(string id, string name, string description, string createddate)
            {
                Id = id; Name = name; Description = description; CreatedDate = createddate;
            }
        }
    public class WorkItem
        {
            public int? id { get; set; }
            //public int rev { get; set; }
            public Fields? fields { get; set; }
            public Links? _links { get; set; }
            public string? url { get; set; }
            public Dictionary<string, string>? TaskCustomFields{ get; set; }=new Dictionary<string, string>();
            public List<WorkItemCommentResponse>? CommentArray { get; set; } = new List<WorkItemCommentResponse>();
        }
        //sagdygy
    public class WorkItemCommentResponse
        {
            public string[] Mentions { get; set; }
            public int? WorkItemId { get; set; }
            public int? Id { get; set; }
            public int? Version { get; set; }
            public string? Text { get; set; }
            public FUser? CreatedBy { get; set; }
            public string? CreatedDate { get; set; }
            public FUser? ModifiedBy { get; set; }
            public string? ModifiedDate { get; set; }
            public string? Format { get; set; }
            public string? RenderedText { get; set; }
            public string? Url { get; set; }
        }
    public class FUser
        {
        public string? displayName { get; set; }
        public string? url { get; set; }
        public Links? _links { get; set; }
        public string? id { get; set; }
        public string? uniqueName { get; set; }
        public string? imageUrl { get; set; }
        public string? descriptor { get; set; }
    }
    public class Links
    {
        public Link? self { get; set; }
        public Link? workItemUpdates { get; set; }
        public Link? workItemRevisions { get; set; }
        public Link? workItemComments { get; set; }
        public Link? html { get; set; }
        public Link? workItemType { get; set; }
        public Link? fields { get; set; }
    }

    public class Link
    {
        public string? href { get; set; }
    }
    public class Fields
        {
            [JsonProperty("System.Title")]
            public string?   System_Title { get; set; }
            [JsonProperty("System.Description")]
            public string? System_Description { get; set; }
            [JsonProperty("System.WorkItemType")]
            public string? System_WorkItemType { get; set; }
            [JsonProperty("System.State")]
            public string? System_State { get; set; }
            [JsonProperty("System.AssignedTo")]
            public FUser?  System_AssignedTo { get; set; }
            [JsonProperty("Microsoft.VSTS.Common.Priority")]
            public int? Microsoft_VSTS_Common_Priority { get; set; }
            //[JsonProperty("System.TeamProject")]
            //public string System_TeamProject { get; set; }
            [JsonProperty("System.Tags")]
            public string? System_Tags { get; set; }

            [JsonProperty("System.Reason")]
            public string? System_Reason { get; set; }
        
            [JsonProperty("System.CreatedDate")]
            public DateTime? System_CreatedDate { get; set; }
            [JsonProperty("System.CreatedBy")]
            public FUser? System_CreatedBy { get; set; }
            [JsonProperty("System.ChangedDate")]
            public DateTime? System_ChangedDate { get; set; }
            [JsonProperty("System.ChangedBy")]
            public FUser? System_ChangedBy { get; set; }
            [JsonProperty("System.CommentCount")]
            public int? System_CommentCount { get; set; }
           

            [JsonProperty("System.BoardColumn")]
            public string? System_BoardColumn { get; set; }
            [JsonProperty("System.BoardColumnDone")]
            public bool? System_BoardColumnDone { get; set; }
            [JsonProperty("Microsoft.VSTS.Common.StateChangeDate")]
            public DateTime? Microsoft_VSTS_Common_StateChangeDate { get; set; }
          
            [JsonProperty("Microsoft.VSTS.Common.Severity")]
            public string? Microsoft_VSTS_Common_Severity { get; set; }
            [JsonProperty("Microsoft.VSTS.Common.ValueArea")]
            public string? Microsoft_VSTS_Common_ValueArea { get; set; }
            [JsonProperty("Microsoft.VSTS.Common.BacklogPriority")]
            public float? Microsoft_VSTS_Common_BacklogPriority { get; set; }
            [JsonProperty("Microsoft.VSTS.TCM.SystemInfo")]
            public string? Microsoft_VSTS_TCM_SystemInfo { get; set; }
            [JsonProperty("Microsoft.VSTS.TCM.ReproSteps")]
            public string? Microsoft_VSTS_TCM_ReproSteps { get; set; }
        }
    public class AssignedTo
    {
        public string? displayName { get; set; }
        public string? url { get; set; }
        public Links? _links { get; set; }
    }
    public class Avatar
    {
        public string? href { get; set; }
    }
    
}

