using ADOCore;
using ADOCore.ApiClietns;
using ADOCore.Models.WiqlQuery;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

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
                queryBuilder.AddCondition(WiqlConsnt.Conjunction.And, WorkItemFields.GetAdoName("AreaPath"), WiqlConsnt.Operator.Under, areaPath);
            if (tag != null)
                queryBuilder.AddCondition(WiqlConsnt.Conjunction.And, "[System.Tags]", WiqlConsnt.Operator.Contains, tag);
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

        public int GetAutomatedTestCountByPriorities(params int[] priorities)
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

        public int GetTestCountByPriority(int priority, params string[] withTags)
        {
            var builder = (WiqlFlatQueryBuilder)(baseQueryBuilder.Clone());
            var baseQuery = builder
                .AddStateCondition("Closed", WiqlConsnt.Operator.NotEqual)
                .AddSinglePriorityCondition(priority, WiqlConsnt.Operator.Equal)
                .AddTitleCondition("a11y", WiqlConsnt.Operator.Contains, WiqlConsnt.Conjunction.AndNot);

            if (withTags.Any())
                foreach (var tag in withTags)
                    baseQuery.AddTagsContainsCondition(tag);

            var query = baseQuery.AddAsOf(asOf).Build();

            var queryResult = wiqlApiClient.PostWiqlQuery(query);

            return queryResult.workItems.Length;
        }

        public int GetTestCount(WiqlFlatQueryBuilder queryBuilder)
        {
            var query = queryBuilder.AddAsOf(asOf).Build();

            var queryResult = wiqlApiClient.PostWiqlQuery(query);
            var ids = string.Join(",", queryResult.workItems.Select(i => i.id));

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
