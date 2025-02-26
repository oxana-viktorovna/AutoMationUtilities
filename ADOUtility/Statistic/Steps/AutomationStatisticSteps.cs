using ADOCore;
using ADOCore.ApiClietns;
using ADOCore.Models.WiqlQuery;
using System;
using System.Collections.Generic;

namespace Statistic.Steps
{
    public class AutomationStatisticSteps
    {
        public AutomationStatisticSteps(
            AdoSettings adoSettings,
            IEnumerable<string> areaPathes,
            DateTime? asOf = null)
        {
            baseQueryBuilder = new WiqlFlatQueryBuilder()
               .AddAttributesToGet("[System.Id]")
               .AddAttributesToGet(WorkItemFields.GetAdoName("Title"))
               .AddAttributesToGet(WorkItemFields.GetAdoName("AreaPath"))
               .AddAttributesToGet(WorkItemFields.GetAdoName("AutomatedBy"))
               .AddTypeCondition("Test Case", conjunction: null);
            wiqlApiClient = new WiqlApiClient(adoSettings);
            this.asOf = asOf;
        }

        private readonly WiqlFlatQueryBuilder baseQueryBuilder;
        private readonly WiqlApiClient wiqlApiClient;
        private readonly DateTime? asOf;

        public int GetNumTests(string areaPath, string tag, int? priority, bool automated = false)
        {
            var queryBuilder = new WiqlFlatQueryBuilder()
                   .AddAttributesToGet("[System.Id]")
                   .AddAttributesToGet(WorkItemFields.GetAdoName("Title"))
                   .AddAttributesToGet(WorkItemFields.GetAdoName("Priority"))
                   .AddAttributesToGet(WorkItemFields.GetAdoName("AutomationStatus"))
                   .AddTypeCondition("Test Case", conjunction: null);
            if (areaPath != null)
                queryBuilder.AddCondition(WorkItemFields.GetAdoName("AreaPath"), areaPath, WiqlConsnt.Operator.Under, WiqlConsnt.Conjunction.And);
            if (tag != null)
                queryBuilder.AddCondition("[System.Tags]",tag, WiqlConsnt.Operator.Contains, WiqlConsnt.Conjunction.And);
            if (priority != null)
                queryBuilder.AddSinglePriorityCondition(priority.Value, WiqlConsnt.Operator.Equal);
            if (automated)
                queryBuilder.AddAutomationStatusCondition("Automated", WiqlConsnt.Operator.Equal);

            queryBuilder.AddAsOf(asOf);

            var query = queryBuilder.Build();

            var queryResult = wiqlApiClient.PostWiqlQuery(query);

            return queryResult.workItems.Length;
        }

        public int GetAutomatedTestCountByPriority(int priority)
        {
            var builder = (WiqlFlatQueryBuilder)baseQueryBuilder.Clone();
            var query = builder
                .AddAutomatedTestsCondition()
                .AddSinglePriorityCondition(priority, WiqlConsnt.Operator.Equal)
                .AddTitleCondition("a11y", WiqlConsnt.Operator.Contains, WiqlConsnt.Conjunction.AndNot)
                .AddAsOf(asOf)
                .Build();

            var queryResult = wiqlApiClient.PostWiqlQuery(query);

            return queryResult.workItems.Length;
        }

        public int GetAutomatedTestCountByPriority(IEnumerable<int> priorities)
        {
            var builder = (WiqlFlatQueryBuilder)baseQueryBuilder.Clone();
            var query = builder
                .AddAutomatedTestsCondition()
                .AddInPriorityCondition(priorities)
                .AddTitleCondition("a11y", WiqlConsnt.Operator.Contains, WiqlConsnt.Conjunction.AndNot)
                .AddAsOf(asOf)
                .Build();

            var queryResult = wiqlApiClient.PostWiqlQuery(query);

            return queryResult.workItems.Length;
        }

        public int GetTestCountByPriority(int priority)
        {
            var builder = (WiqlFlatQueryBuilder)baseQueryBuilder.Clone();
            var query = builder
                .AddStateCondition("Closed", WiqlConsnt.Operator.NotEqual)
                .AddSinglePriorityCondition(priority, WiqlConsnt.Operator.Equal)
                .AddTitleCondition("a11y", WiqlConsnt.Operator.Contains, WiqlConsnt.Conjunction.AndNot)
                .AddAsOf(asOf)
                .Build();

            var queryResult = wiqlApiClient.PostWiqlQuery(query);

            return queryResult.workItems.Length;
        }

        public int GetTestCountByPriority(IEnumerable<int> priorities)
        {
            var builder = (WiqlFlatQueryBuilder)baseQueryBuilder.Clone();
            var query = builder
                .AddStateCondition("Closed", WiqlConsnt.Operator.NotEqual)
                .AddInPriorityCondition(priorities)
                .AddAsOf(asOf)
                .Build();

            var queryResult = wiqlApiClient.PostWiqlQuery(query);

            return queryResult.workItems.Length;
        }
    }
}
