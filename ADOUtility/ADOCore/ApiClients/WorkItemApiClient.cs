﻿using ADOCore.Models;
using RestSharp;
using System;
using System.Collections.Generic;
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
                Op = "test",
                Path = "/rev",
                Value = rev
            };
            var updateTestName = new UpdateWorkItem()
            {
                Op = "add",
                Path = "/fields/Microsoft.VSTS.TCM.AutomatedTestName",
                Value = fullTestName
            };
            var updateStorage = new UpdateWorkItem()
            {
                Op = "add",
                Path = "/fields/Microsoft.VSTS.TCM.AutomatedTestStorage",
                Value = "Tracker.Testing.Automation.Tests.dll"
            };
            var updateTestId = new UpdateWorkItem()
            {
                Op = "add",
                Path = "/fields/Microsoft.VSTS.TCM.AutomatedTestId",
                Value = Guid.NewGuid().ToString()
            };
            var updateTestStatus = new UpdateWorkItem()
            {
                Op = "add",
                Path = "/fields/Microsoft.VSTS.TCM.AutomationStatus",
                Value = "Automated"
            };

            var body = new object[]
                { updateRoot ,
                updateTestName,
                updateTestId,
                updateTestStatus};

            return SendAdoPatchRequest($"wit/workitems/{workItemId}", body, "application/json-patch+json");
        }

        public RestResponse AddTestedByLinksToWorkItem(int workItemId, IEnumerable<int> linkIds, int rev)
        {
            var body = new List<object>();
            var updateRoot = new RootUpdateWorkItem()
            {
                Op = "test",
                Path = "/rev",
                Value = rev
            };
            body.Add(updateRoot);
            foreach (var linkId in linkIds)
            {
                var update = new UpdateWorkItem()
                {
                    Op = "add",
                    Path = "/relations/-",
                    Value = new
                    {
                        rel = "Microsoft.VSTS.Common.TestedBy-Forward",
                        url = $"{baseUrl}/wit/workItems/{linkId}",
                        attributes = new { comment = "Tested by" }
                    }
                };
                body.Add(update);
            }

            return SendAdoPatchRequest($"wit/workitems/{workItemId}", body.ToArray(), "application/json-patch+json");
        }
    }
}
