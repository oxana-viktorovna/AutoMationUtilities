﻿using ADOCore;
using ADOCore.ApiClients;
using ADOCore.Models;
using RestSharp;
using System.Net;

namespace TestRuns.Steps
{
    public class WorkItemApiSteps
    {
        public WorkItemApiClient client { get; private set; }

        public WorkItemApiSteps(AdoSettings adoSettings)
        {
            client = new WorkItemApiClient(adoSettings);
        }

        public WorkItem GetWorkItem(int workItemId)
        => workItemId == 0
            ? null
            : client.GetWorkItem(workItemId);

        public WorkItemNew GetWorkItemNew(int workItemId)
=> workItemId == 0
    ? null
    : client.GetWorkItemNew(workItemId);

        public IRestResponse UpdateAutomationAssociation(int workItemId, string fullTestName)
        {
            var item = GetWorkItem(workItemId);
            return client.UpdateAutomationAssociation(workItemId, fullTestName, item.rev);
        }

        public IRestResponse AddTestedByLinksToWorkItem(int workItemId, IEnumerable<int> linkIds)
        {
            var item = GetWorkItem(workItemId);
            return client.AddTestedByLinksToWorkItem(workItemId, linkIds, item.rev);
        }
    }
}
