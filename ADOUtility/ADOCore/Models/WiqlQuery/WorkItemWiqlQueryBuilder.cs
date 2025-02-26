using System;
using System.Collections.Generic;

namespace ADOCore.Models.WiqlQuery
{
    public class WorkItemWiqlQueryBuilder : ICloneable
    {
        public WorkItemWiqlQueryBuilder()
        {
            query = new WiqlSimpleQuery("workitems");
            query.SelectAttributes = new List<string>();
            query.Conditions = new List<string>();
        }
        private WiqlSimpleQuery query;

        public WorkItemWiqlQueryBuilder AddAsOf(DateTime? date)
        {
            if (date != null)
                AddCondition($"asOf '{date:yyyy-MM-ddTHH:mm:ss.fff}'");

            return this;
        }

        public WorkItemWiqlQueryBuilder AddAttributesToGet(string attribute)
        {
            query.SelectAttributes.Add(attribute);

            return this;
        }

        public WorkItemWiqlQueryBuilder AddAutomatedTestsCondition()
            => AddAutomationStatusCondition("Automated", WiqlConsnt.Operator.Equal)
                .AddAutomatedTestStorageCondition("Tracker.Testing.Automation.Tests.dll", WiqlConsnt.Operator.Equal)
                .AddStateCondition("Closed", WiqlConsnt.Operator.NotEqual);

        public WorkItemWiqlQueryBuilder AddTypeCondition(string type, string comparisonOperator = WiqlConsnt.Operator.Equal, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(WorkItemFields.GetAdoName("Type"), type, comparisonOperator, conjunction);

        public WorkItemWiqlQueryBuilder AddSinglePriorityCondition(int priority, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(WorkItemFields.GetAdoName("Priority"), priority, comparisonOperator, conjunction);

        public WorkItemWiqlQueryBuilder AddInPriorityCondition(IEnumerable<int> priorities, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(WorkItemFields.GetAdoName("Priority"), priorities, WiqlConsnt.Operator.In, conjunction);

        public WorkItemWiqlQueryBuilder AddAutomatedTestStorageCondition(string value, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(WorkItemFields.GetAdoName("AutomatedTestStorage"), value, comparisonOperator, conjunction);

        public WorkItemWiqlQueryBuilder AddAutomationStatusCondition(string automatedStatus, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition(WorkItemFields.GetAdoName("AutomationStatus"), automatedStatus, comparisonOperator, conjunction);

        public WorkItemWiqlQueryBuilder AddTitleCondition(string title, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition("System.Title", title, comparisonOperator, conjunction);

        public WorkItemWiqlQueryBuilder AddStateCondition(string state, string comparisonOperator, string conjunction = WiqlConsnt.Conjunction.And)
            => AddCondition("System.State", state, comparisonOperator, conjunction);

        public WorkItemWiqlQueryBuilder AddCondition(string parameter, string value, string comparisonOperator, string conjunction)
            => AddCondition($"{conjunction} {parameter} {comparisonOperator} '{value}'");
        
        public WorkItemWiqlQueryBuilder AddCondition(string parameter, int value, string comparisonOperator, string conjunction)
            => AddCondition($"{conjunction} {parameter} {comparisonOperator} {value}");

        public WorkItemWiqlQueryBuilder AddCondition(string parameter, IEnumerable<int> values, string comparisonOperator, string conjunction)
            => AddCondition($"{conjunction} {parameter} {comparisonOperator} ({String.Join(",", values)})");

        public WorkItemWiqlQueryBuilder AddCondition(string condition)
        {
            query.Conditions.Add(condition);

            return this;
        }

        public string Build()
            => query.MergedQuery;

        public object Clone()
        {
            var clonned = (WorkItemWiqlQueryBuilder)this.MemberwiseClone();
            clonned.query = (WiqlSimpleQuery)query.Clone();

            return clonned;
        }
    }

    public static class WiqlConsnt
    {
        public static class Conjunction
        {
            public const string And = "AND";
            public const string AndNot = "AND NOT";
            public const string Or = "OR";
            public const string OrNot = "OR NOT";
        }

        public static class Operator
        {
            public const string Equal = "=";
            public const string NotEqual = "<>";
            public const string Greater = ">";
            public const string Less = "<";
            public const string EqualOrGrater = ">=";
            public const string EqualOrLess = "<=";
            public const string In = "IN";
            public const string Contains = "Contains";
            public const string Under = "Under";
        }
    }
}
