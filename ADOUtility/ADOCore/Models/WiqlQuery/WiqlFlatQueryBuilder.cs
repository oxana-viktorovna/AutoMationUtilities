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
            => AddCondition(WorkItemFields.GetAdoName("Type"), type, comparisonOperator, conjunction);

        public WiqlFlatQueryBuilder AddSinglePriorityCondition(int priority, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(WorkItemFields.GetAdoName("Priority"), priority, comparisonOperator, conjunction);

        public WiqlFlatQueryBuilder AddInPriorityCondition(IEnumerable<int> priorities, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(WorkItemFields.GetAdoName("Priority"), priorities, WiqlConsnt.Operator.In, conjunction);

        public WiqlFlatQueryBuilder AddAutomatedTestStorageCondition(string value, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(WorkItemFields.GetAdoName("AutomatedTestStorage"), value, comparisonOperator, conjunction);

        public WiqlFlatQueryBuilder AddAutomationStatusCondition(string automatedStatus, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(WorkItemFields.GetAdoName("AutomationStatus"), automatedStatus, comparisonOperator, conjunction);

        public WiqlFlatQueryBuilder AddTitleCondition(string title, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition("System.Title", title, comparisonOperator, conjunction);

        public WiqlFlatQueryBuilder AddStateCondition(string state, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition("System.State", state, comparisonOperator, conjunction);

        public WiqlFlatQueryBuilder AddTagsContainsCondition(string state, string comparisonOperator = WiqlConsnt.Operator.Contains, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition("System.Tags", state, comparisonOperator, conjunction);

        public WiqlFlatQueryBuilder AddCondition(string parameter, string value, string comparisonOperator, string conjunction)
            => AddCondition($"{conjunction} {parameter} {comparisonOperator} '{value}'");
        
        public WiqlFlatQueryBuilder AddCondition(string parameter, int value, string comparisonOperator, string conjunction)
            => AddCondition($"{conjunction} {parameter} {comparisonOperator} {value}");

        public WiqlFlatQueryBuilder AddCondition(string parameter, IEnumerable<int> values, string comparisonOperator, string conjunction)
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
