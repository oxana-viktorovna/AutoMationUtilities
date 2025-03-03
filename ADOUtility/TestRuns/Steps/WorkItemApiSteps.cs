﻿using ADOCore;
using ADOCore.ApiClients;
using ADOCore.Models;
using RestSharp;

namespace TestRuns.Steps
{
    public class WorkItemApiSteps
    {
        public WorkItemApiClient client { get; private set; }

        public WorkItemApiSteps(AdoSettings adoSettings)
        {
            client = new WorkItemApiClient(adoSettings);
        }

        public WorkItem? GetWorkItem(int workItemId)
        => workItemId == 0
            ? null
            : client.GetWorkItem(workItemId);

        public WorkItemNew? GetWorkItemNew(int workItemId)
        => workItemId == 0
            ? null
            : client.GetWorkItemNew(workItemId);

        public IRestResponse UpdateAutomationAssociation(int workItemId, string fullTestName)
            => client.UpdateAutomationAssociation(workItemId, fullTestName, GetRev(workItemId));

        public IRestResponse AddRelatedLinksToWorkItem(int workItemId, IEnumerable<int> linkIds)
            => client.AddRelatedLinksToWorkItem(workItemId, linkIds, GetRev(workItemId));

        private int GetRev(int workItemId)
        {
            var item = GetWorkItem(workItemId);
            if (item == null)
                throw new NullReferenceException($"Could not get work item {workItemId}");

            return item.rev;
        }
    }
}
