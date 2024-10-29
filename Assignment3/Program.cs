using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TaskClasses;
using Assignment3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Fetching;
using System.Xml.Linq;
using UserClass;
using System.Resources;

class Program
{
    private static readonly string organization = "saurabhgupta0979";
    private static readonly string project = "Assignment3Demo";
    private static readonly string pat = "f7jjuktwxr2lxfj5zyq7qy7damzasx7q35oxihjgcnmr5xjvvdxq";
    public static async Task Main(string[] args)
    {
        await AllProject(); 
    }

    public static async Task AllProject()
    {
        string url = $"https://dev.azure.com/{organization}/_apis/projects?api-version=7.1-preview.4";
        using (HttpClient client = new HttpClient())
        {
            var byteArray = System.Text.Encoding.ASCII.GetBytes($":{pat}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                //Console.WriteLine("aaya"+response);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject jd= JObject.Parse(jsonResponse);

                JArray paresd = (JArray)jd["value"];

                foreach (var val in paresd)
                {
                    Project projectRef = new Project(val["id"].ToString(), val["name"].ToString(), val["description"].ToString(), val["lastUpdateTime"].ToString());
                    projectRef.CustomFields.Add("visibility", val["visibility"].ToString());
                    projectRef.CustomFields.Add("state", val["state"].ToString());
                    await QueryWorkItems(projectRef);
                    await MappingData(projectRef);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
    private static async Task<Project> GetProjectDetails()
    {
        string url = $"https://dev.azure.com/{organization}/_apis/projects/{project}?api-version=2.0";
        using (HttpClient client = new HttpClient())
        {
            var byteArray = System.Text.Encoding.ASCII.GetBytes($":{pat}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Project>(jsonResponse);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }
    }
    private static async Task QueryWorkItems(Project projectDetails)
    {
        string url = $"https://dev.azure.com/{organization}/{project}/_apis/wit/wiql?api-version=7.1-preview.2";

        var queryBody = new
        {
            query = "SELECT [System.Id], [System.Title], [System.State] FROM workitems"
        };

        using (HttpClient client = new HttpClient())
        {
            var byteArray = System.Text.Encoding.ASCII.GetBytes($":{pat}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
          
                var content = new StringContent(JsonConvert.SerializeObject(queryBody), Encoding.UTF8, "application/json");

                // Send POST request
                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

               
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject jObject = JObject.Parse(jsonResponse);
                
                foreach (var workItem in jObject["workItems"])
                {
                    
                    string urlw = (string)workItem["url"];
                    
                  
                    WorkItem workres = await GetWorkItemDetails(urlw,projectDetails);
                    if (workres != null)
                    {
                        projectDetails.TaskArray.Add(workres);
                        
                    }

                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }
    }
    //https://dev.azure.com/saurabhgupta0979/280e5742-c949-43a7-ac6d-5e3515bb5083/_apis/wit/workItems/1
                                                                                 //_apis/wit/workItems/1/comments
    //https://dev.azure.com/saurabhgupta0979/Assignment3Demo/_apis/wit/workItems/1/comments?api-version=4.1-preview.2
    //https://dev.azure.com/saurabhgupta0979/_apis/wit/workItems/1/comments/2
    public static async Task<WorkItem> GetWorkItemDetails(string workItemUrl, Project ProjectName)
    {   
        using (HttpClient client = new HttpClient())
        {
            // Setup Authorization Header with PAT
            var byteArray = System.Text.Encoding.ASCII.GetBytes($":{pat}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Send GET Request
                HttpResponseMessage response = await client.GetAsync(workItemUrl);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject jd=JObject.Parse(jsonResponse);
                string checkString = jd["fields"]?["System.TeamProject"]?.ToString();
                if (checkString== ProjectName.Name)
                {

                    WorkItem workItem = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkItem>(jsonResponse);
                    if (workItem != null)
                    {
                        workItem.TaskCustomFields.Add("System.BoardColumn", jd["fields"]?["System.BoardColumn"]?.ToString());
                        workItem.TaskCustomFields.Add("System.BoardColumnDone", jd["fields"]?["System.BoardColumnDone"]?.ToString());
                        workItem.TaskCustomFields.Add("Microsoft.VSTS.Common.ValueArea", jd["fields"]?["Microsoft.VSTS.Common.ValueArea"]?.ToString());
                        workItem.TaskCustomFields.Add("System.IterationPath", jd["fields"]?["System.IterationPath"]?.ToString());
                    }

                   
                    await GetWorkItemComments(workItemUrl + "/comments",workItem);
                    return workItem;
                }
                return null;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }
    }
    public static async Task GetWorkItemComments(string workurl, WorkItem workItem)
    {
      
        
        using (HttpClient client = new HttpClient())
        {
            var byteArray = System.Text.Encoding.ASCII.GetBytes($":{pat}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(workurl);
                response.EnsureSuccessStatusCode();


                string jsonResponse = await response.Content.ReadAsStringAsync();
              
                JObject jObject = JObject.Parse(jsonResponse);
       
                foreach (var commentItem in jObject["comments"])
                {
                    var CmtObj = JsonConvert.DeserializeObject<WorkItemCommentResponse>(commentItem.ToString());

                    if (CmtObj != null)
                    {
                        workItem.CommentArray.Add(CmtObj);
                        
                    }
                    else
                    {
                        Console.WriteLine("Deserialization failed for a comment.");
                    }

                }

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");

            }
        }
    }
    public static async Task MappingData(Project projectdetails)
    {
        ProjectClass ReturnResponse = new ProjectClass
        {
            id = projectdetails.Id !=null ? projectdetails.Id : "N/A",
            name = projectdetails.Name != null ? projectdetails.Name :"N/A",
            description = projectdetails.Description != null ? projectdetails.Description :"N/A",
            startDate = DateFormat(projectdetails.CreatedDate) != null ? DateFormat(projectdetails.CreatedDate) :"N/A",
            customFields = projectdetails.CustomFields,
            owner = new User { id = "N/A", name = "N/A", email = "N/A" },
            resources = new List<Resource>()
        };
        TaskMapping(projectdetails.TaskArray, ReturnResponse);
        DictonaryPro d=new DictonaryPro();
        if (d != null)
        {
            d.ProjectJson.Add($"{projectdetails.Name}", ReturnResponse);
            //obj.ProjectJson.Add("key", "value");
        }
        
        Console.WriteLine(JsonConvert.SerializeObject(d.ProjectJson, Formatting.Indented));
        //Console.WriteLine(JsonConvert.SerializeObject(ReturnResponse));
    }
    public static async Task TaskMapping(List<WorkItem> items, ProjectClass pro)
    {
        foreach (var item in items)
        {
            if (item?.fields == null)
            {
                Console.WriteLine("fields object is null");
                return;
            }

            TaskItem TaskObj = new TaskItem
            {
                id = item.id.ToString(),
                title = item.fields.System_Title,
                description = item.fields.System_Description,
                type = item.fields.System_WorkItemType,
                status = item.fields.System_State,
                customFields = item.TaskCustomFields,
                assignees = item.fields.System_AssignedTo != null
                       ? new User
                       {
                           id = item.fields.System_AssignedTo.id,
                           name = item.fields.System_AssignedTo.displayName,
                           email = item.fields.System_AssignedTo.uniqueName
                       }
                       : null,
                reporters = item.fields.System_CreatedBy != null
                       ? new User
                       {
                           id = item.fields.System_CreatedBy.id,
                           name = item.fields.System_CreatedBy.displayName,
                           email = item.fields.System_CreatedBy.uniqueName
                       }
                       : null,
                priority = item.fields.Microsoft_VSTS_Common_Priority.ToString(),
                tags = GetAllTag(item.fields.System_Tags?.ToString()),
                startDate = DateFormat(item.fields.System_ChangedDate.ToString()),
                comments = AllComment(item.CommentArray),
                subtasks = new List<Subtask>(),
                attachments=new List<Attachment>(),
                timelogs = new List<Timelog>(),
            };
            if (TaskObj != null)
            {
                pro.Tasks.Add(TaskObj);
                //Console.WriteLine(pro.Tasks.Id);
            }
            else
            {
                Console.WriteLine("null");
            }

        }
    }
    public static List<Comment> AllComment(List<WorkItemCommentResponse> allComment)
    {
        List<Comment> RetunAllComment = new List<Comment>();

        foreach (var com in allComment)
        {
            
            string id = com.Id?.ToString() ?? "N/A";
            string text = com.Text ?? "N/A";
            string authorId = com.CreatedBy?.id ?? "N/A";
            string authorName = com.CreatedBy?.displayName ?? "N/A";
            string authorEmail = com.CreatedBy?.uniqueName ?? "N/A";
            string timestamp = DateTimeFormat(com.CreatedDate?.ToString() ?? "N/A");

         
            Author author = new Author(authorId, authorName, authorEmail);
            Comment comment = new Comment(id, text, author, timestamp);

            RetunAllComment.Add(comment);
        }
        return RetunAllComment;
    }
    public static string DateTimeFormat(string format)
    {
        DateTime dateTime = DateTime.Parse(format);
        string formattedDate = dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
        return formattedDate;
    }
    public static string DateFormat(string format)
    {
        DateTime dateTime = DateTime.Parse(format);
        string formattedDate = dateTime.ToString("yyyy-MM-dd");
        return formattedDate;
    }
    public static List<string> GetAllTag(string tag)
    {
        List<string> Tags = new List<string>();
        int i = 0;
        string emp = "";
        if(tag == null) { return null; }
        int leng = tag.Length;
        while (leng > i)
        {
            if (tag[i]!=';')
            {
                emp = emp + tag[i++];
            }
            else
            {
                Tags.Add(emp);
                emp = "";
                i = i + 2;
            }
        }
        Tags.Add(emp);
        return Tags;
    }
}
