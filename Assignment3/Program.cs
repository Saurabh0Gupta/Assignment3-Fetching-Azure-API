using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Assignment3_version_1._0._0._0;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Fetching;
using static Fetching.FetchingPro;
using System.Xml.Linq;

class Program
{
    private static readonly string organization = "saurabhgupta0979";
    private static readonly string project = "Assignment3Demo";
    private static readonly string pat = "2o5mwbcpg2jsih2nrfyxu7vobfytqzlfnu55bmtotsbuzxqptm7a"; // Your PAT

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
                // Prepare the content
                var content = new StringContent(JsonConvert.SerializeObject(queryBody), Encoding.UTF8, "application/json");

                // Send POST request
                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                // Read the response content
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject jObject = JObject.Parse(jsonResponse);
                // Access the "workItems" array
                foreach (var workItem in jObject["workItems"])
                {
                    //int id = (int)workItem["id"];
                    
                    string urlw = (string)workItem["url"];
                    
                    //Console.WriteLine($"Work Item ID: {id}, URL: {urlw}");
                    //Console.WriteLine(urlw);
                    WorkItem workres = await GetWorkItemDetails(urlw,projectDetails);
                    if (workres != null)
                    {
                        //Console.WriteLine(workres.id);
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
                // Send GET Request to the provided Work Item URL
                HttpResponseMessage response = await client.GetAsync(workItemUrl);
                response.EnsureSuccessStatusCode();

                // Parse the response content
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

                    //Console.WriteLine($"Work Item ID: {workItem.id}, Title: {workItem.fields.System_Tags}, State: {workItem.fields.System_Title}");
                    //Console.WriteLine(keyValuePairs["fields"]);
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
        //Console.WriteLine(workurl);
        //Console.WriteLine(workurl+"/comments");
        //string url = $"https://dev.azure.com/saurabhgupta0979/Assignment3Demo/_apis/wit/workItems/{workItemId}/comments?api-version=4.1-preview.2";

        using (HttpClient client = new HttpClient())
        {
            // Set up the Authorization Header
            var byteArray = System.Text.Encoding.ASCII.GetBytes($":{pat}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(workurl);
                response.EnsureSuccessStatusCode();


                string jsonResponse = await response.Content.ReadAsStringAsync();
              
                JObject jObject = JObject.Parse(jsonResponse);
       
                //foreach (var commentItem in jObject["comments"])
                //{
                foreach (var commentItem in jObject["comments"])
                {
                    var CmtObj = JsonConvert.DeserializeObject<WorkItemCommentResponse>(commentItem.ToString());

                    if (CmtObj != null)
                    {
                        workItem.CommentArray.Add(CmtObj);
                        //Console.WriteLine(workItem.CommentArray[0].Id);
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
        Project2 ReturnResponse = new Project2
        {
            Id = projectdetails.Id,
            Name = projectdetails.Name,
            Description = projectdetails.Description,
            CreatedDate = DateFormat(projectdetails.CreatedDate),
            customFields = projectdetails.CustomFields
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
    public static async Task TaskMapping(List<WorkItem> items, Project2 pro)
    {
        foreach (var item in items)
        {
            //Console.WriteLine(item.id);
            //Console.WriteLine(item.fields.System_Title);
            //Console.WriteLine(item.fields.System_Description);
            //Console.WriteLine(item.fields.System_WorkItemType);
            //Console.WriteLine(item.fields.System_State);
            //Console.WriteLine(item.fields.Microsoft_VSTS_Common_Priority.ToString());
            //Console.WriteLine(item.fields.System_Tags?.ToString());
            //Console.WriteLine(item.fields.System_ChangedDate);
            if (item?.fields == null)
            {
                Console.WriteLine("fields object is null");
                return;
            }

            TaskItem TaskObj = new TaskItem
            {
                Id = item.id.ToString(),
                Title = item.fields.System_Title,
                Description = item.fields.System_Description,
                Type = item.fields.System_WorkItemType,
                Status = item.fields.System_State,
                CustomFields_task=item.TaskCustomFields,
                Assignees = item.fields.System_AssignedTo != null
                       ? new Assignee
                       {
                           Id = item.fields.System_AssignedTo.id,
                           Name = item.fields.System_AssignedTo.displayName,
                           Email = item.fields.System_AssignedTo.uniqueName
                       }
                       : null,
                Reporters = item.fields.System_CreatedBy != null
                       ? new Reporter
                       {
                           Id = item.fields.System_CreatedBy.id,
                           Name = item.fields.System_CreatedBy.displayName,
                           Email = item.fields.System_CreatedBy.uniqueName
                       }
                       : null,
                Priority = item.fields.Microsoft_VSTS_Common_Priority.ToString(),
                Tags =  GetAllTag(item.fields.System_Tags?.ToString()),
                StartDate = DateFormat(item.fields.System_ChangedDate.ToString()),
                Comments = AllComment(item.CommentArray)

            };
            if (TaskObj != null)
            {

                pro.Tasks.Add(TaskObj);
                //Console.WriteLine(pro.Tasks.Id);
            }
            else
            {
                Console.WriteLine("null");
                //return;
            }
            
            //Console.WriteLine(JsonConvert.SerializeObject("aa"));

        }
    }
    //public static List<Comment> AllComment(List<WorkItemCommentResponse> allComment)
    //{
    //    List<Comment> RetunAllComment = new List<Comment>();

    //    foreach (var com in allComment)
    //    {
    //        Console.WriteLine(com.Id.ToString());
    //        //JObject jd = JObject.Parse(com.ToString());
    //        Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaaa" );
    //        RetunAllComment.Add(new Comment(com.Id.ToString(), com.Id.ToString(), new Author(com.Id.ToString(), com.Id.ToString(), com.Id.ToString()), DateTimeFormat(com.Id.ToString())));
    //    }
    //    return RetunAllComment;
    //}
    public static List<Comment> AllComment(List<WorkItemCommentResponse> allComment)
    {
        List<Comment> RetunAllComment = new List<Comment>();

        foreach (var com in allComment)
        {
            // Ensure we are accessing the correct properties from WorkItemCommentResponse
            string id = com.Id?.ToString() ?? "Unknown";
            string text = com.Text ?? "No Text";
            string authorId = com.CreatedBy?.id ?? "Unknown";
            string authorName = com.CreatedBy?.displayName ?? "Unknown";
            string authorEmail = com.CreatedBy?.uniqueName ?? "No Email";
            string timestamp = DateTimeFormat(com.CreatedDate?.ToString() ?? DateTime.Now.ToString());

            // Create the Author and Comment objects
            Author author = new Author(authorId, authorName, authorEmail);
            Comment comment = new Comment(id, text, author, timestamp);

            // Add the comment to the list
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
