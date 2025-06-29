using ADOCore.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ADOCore.ApiClients
{
    public class WorkItemApiClient : CoreAdoApiClient
    {
        public WorkItemApiClient(AdoSettings adoSettings) : base(adoSettings)
        {
        }

        public WorkItem GetWorkItem(int workItemId)
        {
            var response = SendAdoRequest($"wit/workitems/{workItemId}", Method.Get);

            return JsonSerializer.Deserialize<WorkItem>(response.Content);
        }

        public RestResponse UpdateAutomationAssociation(int workItemId, string fullTestName, int rev)
        {
            var updateRoot = new RootUpdateWorkItem()
            {
                op = "test",
                path = "/rev",
                value = rev
            };
            var updateTestId = new UpdateWorkItem()
            {
                op = "add",
                path = "/fields/Microsoft.VSTS.TCM.AutomatedTestId",
                value = "32"//Guid.NewGuid().ToString()
            };
            var updateTestName = new UpdateWorkItem()
            {
                op = "add",
                path = "/fields/Microsoft.VSTS.TCM.AutomatedTestName",
                value = fullTestName
            };
            var updateStorage = new UpdateWorkItem()
            {
                op = "add",
                path = "/fields/Microsoft.VSTS.TCM.AutomatedTestStorage",
                value = "Tracker.Testing.Automation.Tests.dll"
            };
            var updateTestStatus = new UpdateWorkItem()
            {
                op = "add",
                path = "/fields/Microsoft.VSTS.TCM.AutomationStatus",
                value = "Automated"
            };

            var body = new object[]
                { updateRoot ,
                updateTestName,
                updateStorage,
                updateTestId,
                updateTestStatus};

            return SendAdoPatchRequest($"wit/workitems/{workItemId}", body, "application/json-patch+json");
        }

        public RestResponse AddRelatedLinksToWorkItem(int workItemId, IEnumerable<int> linkIds, int rev)
        {
            var body = new List<object>();
            var updateRoot = new RootUpdateWorkItem()
            {
                op = "test",
                path = "/rev",
                value = rev
            };
            body.Add(updateRoot);
            foreach (var linkId in linkIds)
            {
                var update = new UpdateWorkItem()
                {
                    op = "add",
                    path = "/relations/-",
                    value = new
                    {
                        rel = "System.LinkTypes.Related",
                        url = $"{baseUrl}/wit/workItems/{linkId}",
                        attributes = new { comment = "Related" }
                    }
                };
                body.Add(update);
            }

            return SendAdoPatchRequest($"wit/workitems/{workItemId}", body.ToArray(), "application/json-patch+json");
        }
    }
}
