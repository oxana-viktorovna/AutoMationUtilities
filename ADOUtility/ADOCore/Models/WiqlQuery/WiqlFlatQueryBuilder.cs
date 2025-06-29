using System;
using System.Collections.Generic;

namespace ADOCore.Models.WiqlQuery
{
    public class WiqlFlatQueryBuilder : ICloneable
    {
        public WiqlFlatQueryBuilder()
        {
            query = new WiqlFlatQuery();
            query.SelectAttributes = new List<string>();
            query.Conditions = new List<string>();
        }

        private WiqlFlatQuery query;

        public WiqlFlatQueryBuilder AddAsOf(DateTime? date)
        {
            if (date != null)
                AddCondition($"asOf '{date:yyyy-MM-ddTHH:mm:ss.fff}'");

            return this;
        }

        public WiqlFlatQueryBuilder AddAttributesToGet(string attribute)
        {
            query.SelectAttributes.Add(attribute);

            return this;
        }

        public WiqlFlatQueryBuilder AddAutomatedTestsCondition()
            => AddAutomationStatusCondition("Automated", WiqlConsnt.Operator.Equal)
                .AddAutomatedTestStorageCondition("Tracker.Testing.Automation.Tests.dll", WiqlConsnt.Operator.Equal)
                .AddStateCondition("Closed", WiqlConsnt.Operator.NotEqual);

        public WiqlFlatQueryBuilder AddTypeCondition(string type, string comparisonOperator = WiqlConsnt.Operator.Equal, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(conjunction, WorkItemFields.GetAdoName("Type"), comparisonOperator, type);

        public WiqlFlatQueryBuilder AddSinglePriorityCondition(int priority, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(conjunction, WorkItemFields.GetAdoName("Priority"), comparisonOperator, priority);

        public WiqlFlatQueryBuilder AddInPriorityCondition(IEnumerable<int> priorities, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(conjunction, WorkItemFields.GetAdoName("Priority"), WiqlConsnt.Operator.In, priorities);

        public WiqlFlatQueryBuilder AddAutomatedTestStorageCondition(string value, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(conjunction, WorkItemFields.GetAdoName("AutomatedTestStorage"), comparisonOperator, value);

        public WiqlFlatQueryBuilder AddAutomationStatusCondition(string automatedStatus, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(conjunction, WorkItemFields.GetAdoName("AutomationStatus"), comparisonOperator, automatedStatus);

        public WiqlFlatQueryBuilder AddTitleCondition(string title, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(conjunction, "System.Title", comparisonOperator, title);

        public WiqlFlatQueryBuilder AddStateCondition(string state, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(conjunction, "System.State", comparisonOperator, state);

        public WiqlFlatQueryBuilder AddTagsContainsCondition(string state, string comparisonOperator = WiqlConsnt.Operator.Contains, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(conjunction, "System.Tags", comparisonOperator, state);

        public WiqlFlatQueryBuilder AddCondition(string conjunction, string parameter, string comparisonOperator,string value)
            => AddCondition($"{conjunction} {parameter} {comparisonOperator} '{value}'");

        public WiqlFlatQueryBuilder AddCondition(string conjunction, string parameter, string comparisonOperator, int value)
            => AddCondition($"{conjunction} {parameter} {comparisonOperator} {value}");

        public WiqlFlatQueryBuilder AddCondition(string conjunction, string parameter, string comparisonOperator, IEnumerable<int> values)
            => AddCondition($"{conjunction} {parameter} {comparisonOperator} ({String.Join(",", values)})");

        public WiqlFlatQueryBuilder AddCondition(string condition)
        {
            query.Conditions.Add(condition);

            return this;
        }

        public string Build()
            => query.MergedQuery;

        public object Clone()
        {
            var clonned = (WiqlFlatQueryBuilder)this.MemberwiseClone();
            clonned.query = (WiqlFlatQuery)query.Clone();

            return clonned;
        }
    }
}
