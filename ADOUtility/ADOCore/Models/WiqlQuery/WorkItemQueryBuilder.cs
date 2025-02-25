using System;
using System.Collections.Generic;

namespace ADOCore.Models.WiqlQuery
{
    internal class WorkItemQueryBuilder : ICloneable
    {
        public WorkItemQueryBuilder()
        {
            query = new WiqlSimpleQuery("workitems");
            query.SelectAttributes = new List<string>();
            query.Conditions = new List<string>();
        }
        private WiqlSimpleQuery query;

        internal WorkItemQueryBuilder AddAsOf(DateTime? date)
        {
            if (date != null)
                query.Conditions.Add($"asOf '{date:yyyy-MM-ddTHH:mm:ss.fff}'");

            return this;
        }

        internal WorkItemQueryBuilder AddAttributesToGet(string attribute)
        {
            query.SelectAttributes.Add(attribute);

            return this;
        }

        internal WorkItemQueryBuilder AddConditions(string condition)
        {
            query.Conditions.Add(condition);

            return this;
        }

        internal WorkItemQueryBuilder AddAutomatedTestsCondition()
            => AddAutomationStatusCondition("Automated", "=")
                .AddAutomatedTestStorageCondition("Tracker.Testing.Automation.Tests.dll", "=")
                .AddStateCondition("Closed", "<>");

        internal WorkItemQueryBuilder AddSinglePriorityCondition(int priority, string comparisonOperator, string conjunction = "AND")
            => AddSingleCondition("Microsoft.VSTS.Common.Priority", priority, comparisonOperator, conjunction);

        internal WorkItemQueryBuilder AddInPriorityCondition(IEnumerable<int> priorities, string conjunction = "AND")
            => AddSingleCondition("Microsoft.VSTS.Common.Priority", priorities, "IN", conjunction);

        internal WorkItemQueryBuilder AddAutomatedTestStorageCondition(string value, string comparisonOperator, string conjunction = "AND")
            => AddSingleCondition("Microsoft.VSTS.TCM.AutomatedTestStorage", value, comparisonOperator, conjunction);

        internal WorkItemQueryBuilder AddAutomationStatusCondition(string automatedStatus, string comparisonOperator, string conjunction = "AND")
            => AddSingleCondition("Microsoft.VSTS.TCM.AutomationStatus", automatedStatus, comparisonOperator, conjunction);

        internal WorkItemQueryBuilder AddTitleCondition(string title, string comparisonOperator, string conjunction = "AND")
            => AddSingleCondition("System.Title", title, comparisonOperator, conjunction);

        internal WorkItemQueryBuilder AddStateCondition(string state, string comparisonOperator, string conjunction = "AND")
            => AddSingleCondition("System.State", state, comparisonOperator, conjunction);

        internal WorkItemQueryBuilder AddSingleCondition(string parameter, string value, string comparisonOperator, string conjunction)
        {
            query.Conditions.Add($"{conjunction} [{parameter}] {comparisonOperator} '{value}'");

            return this;
        }

        internal WorkItemQueryBuilder AddSingleCondition(string parameter, int value, string comparisonOperator, string conjunction)
        {
            query.Conditions.Add($"{conjunction} [{parameter}] {comparisonOperator} {value}");

            return this;
        }

        internal WorkItemQueryBuilder AddSingleCondition(string parameter, IEnumerable<int> values, string comparisonOperator, string conjunction)
        {
            query.Conditions.Add($"{conjunction} [{parameter}] {comparisonOperator} ({String.Join(",", values)})");

            return this;
        }

        internal string Build()
            => query.MergedQuery;

        public object Clone()
        {
            var clonned = (WorkItemQueryBuilder)this.MemberwiseClone();
            clonned.query = (WiqlSimpleQuery)query.Clone();

            return clonned;
        }
    }
}
