
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text.Json.Serialization;
using static NPOI.HSSF.UserModel.HeaderFooter;

public class WorkItem
{
    public int id { get; set; }
    public int rev { get; set; }
    public WorkItemFields fields { get; set; }
    public _Links5 _links { get; set; }
    public string url { get; set; }
}

public class WorkItemFields
{
    [JsonPropertyName("System.AreaPath")]
    public string AreaPath { get; set; }

    [JsonPropertyName("System.TeamProject")]
    public string Project { get; set; }

    [JsonPropertyName("System.IterationPath")]
    public string IterationPath { get; set; }

    [JsonPropertyName("System.WorkItemType")]
    public string Type { get; set; }

    [JsonPropertyName("System.State")]
    public string State { get; set; }

    [JsonPropertyName("System.Reason")]
    public string Reason { get; set; }

    [JsonPropertyName("System.AssignedTo")]
    public SystemAssignedto AssignedTo { get; set; }

    [JsonPropertyName("System.CreatedDate")]
    public DateTime CreatedDate { get; set; }

    [JsonPropertyName("System.CreatedBy")]
    public SystemCreatedby CreatedBy { get; set; }

    [JsonPropertyName("System.ChangedDate")]
    public DateTime ChangedDate { get; set; }

    [JsonPropertyName("System.ChangedBy")]
    public SystemChangedby ChangedBy { get; set; }

    [JsonPropertyName("System.CommentCount")]
    public int CommentCount { get; set; }

    [JsonPropertyName("System.Title")]
    public string Title { get; set; }

    [JsonPropertyName("Microsoft.VSTS.Common.StateChangeDate")]
    public DateTime StateChangeDate { get; set; }

    [JsonPropertyName("Microsoft.VSTS.Common.ActivatedDate")]
    public DateTime ActivatedDate { get; set; }

    [JsonPropertyName("Microsoft.VSTS.Common.ActivatedBy")]
    public MicrosoftVSTSCommonActivatedby ActivatedBy { get; set; }

    [JsonPropertyName("Microsoft.VSTS.Common.Priority")]
    public int Priority { get; set; }

    [JsonPropertyName("Microsoft.VSTS.TCM.AutomatedTestName")]
    public string AutomatedTestName { get; set; }

    [JsonPropertyName("Microsoft.VSTS.TCM.AutomatedTestStorage")]
    public string AutomatedTestStorage { get; set; }

    [JsonPropertyName("Microsoft.VSTS.TCM.AutomatedTestId")]
    public string AutomatedTestId { get; set; }

    [JsonPropertyName("Microsoft.VSTS.TCM.AutomationStatus")]
    public string AutomationStatus { get; set; }

    [JsonPropertyName("TR.Elite.AutomatedBy")]
    public TREliteAutomatedby AutomatedBy { get; set; }

    [JsonPropertyName("TR.Elite.AutomationPriority")]
    public int AutomationPriority { get; set; }

    [JsonPropertyName("TR.Elite.FunctionalAreaPath")]
    public string FuncAreaPath { get; set; }

    [JsonPropertyName("Custom.Category")]
    public string CustomCategory { get; set; }

    [JsonPropertyName("Microsoft.VSTS.TCM.Steps")]
    public string Steps { get; set; }

    public static string GetAdoName(string field)
    {
        var fieldsType = typeof(WorkItemFields);
        var propertyInfo = fieldsType.GetProperty(field);
        var attribute = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();

        return attribute != null ? "["+attribute.Name+"]" : null;
    }
}

public class SystemAssignedto
{
    public string displayName { get; set; }
    public string url { get; set; }
    public _Links _links { get; set; }
    public string id { get; set; }
    public string uniqueName { get; set; }
    public string imageUrl { get; set; }
    public string descriptor { get; set; }
}

public class _Links
{
    public Avatar avatar { get; set; }
}

public class Avatar
{
    public string href { get; set; }
}

public class SystemCreatedby
{
    public string displayName { get; set; }
    public string url { get; set; }
    public _Links1 _links { get; set; }
    public string id { get; set; }
    public string uniqueName { get; set; }
    public string imageUrl { get; set; }
    public string descriptor { get; set; }
}

public class _Links1
{
    public Avatar1 avatar { get; set; }
}

public class Avatar1
{
    public string href { get; set; }
}

public class SystemChangedby
{
    public string displayName { get; set; }
    public string url { get; set; }
    public _Links2 _links { get; set; }
    public string id { get; set; }
    public string uniqueName { get; set; }
    public string imageUrl { get; set; }
    public string descriptor { get; set; }
}

public class _Links2
{
    public Avatar2 avatar { get; set; }
}

public class Avatar2
{
    public string href { get; set; }
}

public class MicrosoftVSTSCommonActivatedby
{
    public string displayName { get; set; }
    public string url { get; set; }
    public _Links3 _links { get; set; }
    public string id { get; set; }
    public string uniqueName { get; set; }
    public string imageUrl { get; set; }
    public string descriptor { get; set; }
}

public class _Links3
{
    public Avatar3 avatar { get; set; }
}

public class Avatar3
{
    public string href { get; set; }
}

public class TREliteAutomatedby
{
    public string displayName { get; set; }
    public string url { get; set; }
    public _Links4 _links { get; set; }
    public string id { get; set; }
    public string uniqueName { get; set; }
    public string imageUrl { get; set; }
    public string descriptor { get; set; }
}

public class _Links4
{
    public Avatar4 avatar { get; set; }
}

public class Avatar4
{
    public string href { get; set; }
}

public class _Links5
{
    public Self self { get; set; }
    public Workitemupdates workItemUpdates { get; set; }
    public Workitemrevisions workItemRevisions { get; set; }
    public Workitemcomments workItemComments { get; set; }
    public Html html { get; set; }
    public Workitemtype workItemType { get; set; }
    public Fields1 fields { get; set; }
}

public class Self
{
    public string href { get; set; }
}

public class Workitemupdates
{
    public string href { get; set; }
}

public class Workitemrevisions
{
    public string href { get; set; }
}

public class Workitemcomments
{
    public string href { get; set; }
}

public class Html
{
    public string href { get; set; }
}

public class Workitemtype
{
    public string href { get; set; }
}

public class Fields1
{
    public string href { get; set; }
}
