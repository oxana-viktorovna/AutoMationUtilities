using ADOCore;
using ADOCore.ApiClietns;
using Statistic.Query;
using System;
using System.Collections.Generic;

namespace Statistic.Steps
{
    internal class AutomationStatisticSteps
    {
        public AutomationStatisticSteps(
            AdoSettings adoSettings, 
            IEnumerable<string> areaPathes, 
            DateTime? asOf = null)
        {
            baseQueryBuilder = new WorkItemQueryBuilder()
               .AddAttributesToGet("[System.Id]")
               .AddAttributesToGet("[System.Title]")
               .AddConditions("[System.TeamProject] = @project")
               .AddConditions("AND [System.WorkItemType] = 'Test Case'")
               .AddConditions("AND [System.State] <> 'Closed'")
               .AddAreaPathConditions(areaPathes, true);
            wiqlApiClient = new WiqlApiClient(adoSettings);
            this.asOf = asOf;
        }

        private readonly WorkItemQueryBuilder baseQueryBuilder;
        private readonly WiqlApiClient wiqlApiClient;
        private readonly DateTime? asOf;

        internal int GetAutomatedTestCountByPriority(int priority)
        {
            var builder = (WorkItemQueryBuilder)baseQueryBuilder.Clone();
            var query = builder
                .AddAutomationStatusCondition("Automated", "=")
                .AddAutomatedTestStorageCondition("Tracker.Testing.Automation.Tests.dll", "=")
                .AddSinglePriorityCondition(priority, "=")
                .AddAsOf(asOf)
                .Build();

            var queryResult = wiqlApiClient.PostWiqlQuery(query);

            return queryResult.workItems.Length;
        }

        public int GetNumTests(string areaPath, string tag, int? priority, bool automated = false)
        {
            var queryBuilder = new WorkItemQueryBuilder()
                   .AddAttributesToGet("[System.Id]")
                   .AddAttributesToGet("[System.Title]")
                   .AddAttributesToGet("[Microsoft.VSTS.Common.Priority]")
                   .AddAttributesToGet("[Microsoft.VSTS.TCM.AutomationStatus]")
                   .AddConditions("[System.TeamProject] = @project")
                   .AddConditions("AND [System.WorkItemType] = 'Test Case'");
            if (areaPath != null)
                queryBuilder.AddConditions($"AND [System.AreaPath] UNDER '{areaPath}'");
            if (tag != null)
                queryBuilder.AddConditions($"AND [System.Tags] CONTAINS '{tag}'");
            if (priority != null)
                queryBuilder.AddSinglePriorityCondition(priority.Value, "=");
            if (automated)
                queryBuilder.AddAutomationStatusCondition("Automated", "=");

            var query = queryBuilder.AddAsOf(asOf)
                   .Build();

            var queryResult = wiqlApiClient.PostWiqlQuery(query);

            return queryResult.workItems.Length;
        }

        internal int GetAutomatedTestCountByPriority(IEnumerable<int> priorities)
        {
            var builder = (WorkItemQueryBuilder)baseQueryBuilder.Clone();
            var query = builder
                .AddAutomationStatusCondition("Automated", "=")
                .AddAutomatedTestStorageCondition("Tracker.Testing.Automation.Tests.dll", "=")
                .AddInPriorityCondition(priorities)
                .AddAsOf(asOf)
                .Build();

            var queryResult = wiqlApiClient.PostWiqlQuery(query);

            return queryResult.workItems.Length;
        }

        internal int GetTestCountByPriority(int priority)
        {
            var builder = (WorkItemQueryBuilder)baseQueryBuilder.Clone();
            var query = builder
                .AddSinglePriorityCondition(priority, "=")
                .AddAsOf(asOf)
                .Build();

            var queryResult = wiqlApiClient.PostWiqlQuery(query);

            return queryResult.workItems.Length;
        }

        internal int GetTestCountByPriority(IEnumerable<int> priorities)
        {
            var builder = (WorkItemQueryBuilder)baseQueryBuilder.Clone();
            var query = builder
                .AddInPriorityCondition(priorities)
                .AddAsOf(asOf)
                .Build();

            var queryResult = wiqlApiClient.PostWiqlQuery(query);

            return queryResult.workItems.Length;
        }
    }
}
