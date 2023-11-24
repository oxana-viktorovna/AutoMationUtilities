using System;
using System.Collections.Generic;

namespace ADOCore.Models
{
    public class WorkItem
    {
        public int id { get; set; }
        public int rev { get; set; }
        public Fields fields { get; set; }
        public _Links5 _links { get; set; }
        public string url { get; set; }
        public string Name { get; set; }
        public List<WorkItemField> WorkItemFields { get; set; }
    }

    public class Fields
    {
        public string SystemAreaPath { get; set; }
        public string SystemTeamProject { get; set; }
        public string SystemIterationPath { get; set; }
        public string SystemWorkItemType { get; set; }
        public string SystemState { get; set; }
        public string SystemReason { get; set; }
        public SystemAssignedto SystemAssignedTo { get; set; }
        public DateTime SystemCreatedDate { get; set; }
        public SystemCreatedby SystemCreatedBy { get; set; }
        public DateTime SystemChangedDate { get; set; }
        public SystemChangedby SystemChangedBy { get; set; }
        public int SystemCommentCount { get; set; }
        public string SystemTitle { get; set; }
        public DateTime MicrosoftVSTSCommonStateChangeDate { get; set; }
        public DateTime MicrosoftVSTSCommonActivatedDate { get; set; }
        public MicrosoftVSTSCommonActivatedby MicrosoftVSTSCommonActivatedBy { get; set; }
        public int MicrosoftVSTSCommonPriority { get; set; }
        public string MicrosoftVSTSTCMAutomatedTestName { get; set; }
        public string MicrosoftVSTSTCMAutomatedTestStorage { get; set; }
        public string MicrosoftVSTSTCMAutomatedTestId { get; set; }
        public string MicrosoftVSTSTCMAutomationStatus { get; set; }
        public string TREliteRunFrequency { get; set; }
        public TREliteAutomatedby TREliteAutomatedBy { get; set; }
        public string MicrosoftVSTSTCMSteps { get; set; }
        public string MicrosoftVSTSTCMLocalDataSource { get; set; }
        public string SystemTags { get; set; }
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

    public class SystemCreatedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public bool inactive { get; set; }
        public string descriptor { get; set; }
    }

    public class SystemChangedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class MicrosoftVSTSCommonActivatedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public bool inactive { get; set; }
        public string descriptor { get; set; }
    }

    public class TREliteAutomatedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
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
}
