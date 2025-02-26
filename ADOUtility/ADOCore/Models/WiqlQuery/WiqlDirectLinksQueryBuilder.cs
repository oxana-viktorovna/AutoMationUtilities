using System;
using System.Collections.Generic;

namespace ADOCore.Models.WiqlQuery
{
    public class WiqlDirectLinksQueryBuilder : ICloneable
    {
        public WiqlDirectLinksQueryBuilder()
        {
            query = new WiqlDirectLinksQuery();
            query.SelectAttributes = new List<string>();
            query.SourceConditions = new List<string>();
            query.TargetConditions = new List<string>();
            query.Mode = "MustContain";
        }

        private WiqlDirectLinksQuery query;
        public string Build()
            => query.MergedQuery;

        public object Clone()
        {
            var clonned = (WiqlDirectLinksQueryBuilder)this.MemberwiseClone();
            clonned.query = (WiqlDirectLinksQuery)query.Clone();

            return clonned;
        }

        public WiqlDirectLinksQueryBuilder AddAttributesToGet(string attribute)
        {
            query.SelectAttributes.Add(attribute);

            return this;
        }

        #region target

        public WiqlDirectLinksQueryBuilder AddTargetCondition(string conjunction, string parameter, string comparisonOperator, string value)
        {
            var valueToSet = comparisonOperator == WiqlConsnt.Operator.In ? value : $"'{value}'";

            return AddTargetCondition($"{conjunction} [Target].{parameter} {comparisonOperator} {valueToSet}"); 
        }

        public WiqlDirectLinksQueryBuilder AddTargetCondition(string conjunction, string parameter, string comparisonOperator, int? value)
            => AddTargetCondition($"{conjunction} [Target].{parameter} {comparisonOperator} {value}");

        public WiqlDirectLinksQueryBuilder AddTargetCondition(string conjunction, string parameter, string comparisonOperator, IEnumerable<int> values)
            => AddTargetCondition($"{conjunction} [Target].{parameter} {comparisonOperator} ({String.Join(",", values)})");

        public WiqlDirectLinksQueryBuilder AddTargetCondition(string condition)
        {
            query.TargetConditions.Add(condition);

            return this;
        }

        #endregion target

        #region source

        public WiqlDirectLinksQueryBuilder AddSourceCondition(string conjunction, string parameter, string comparisonOperator, string value)
        {
            var valueToSet = comparisonOperator == WiqlConsnt.Operator.In ? value : $"'{value}'";

            return AddSourceCondition($"{conjunction} [Source].{parameter} {comparisonOperator} {valueToSet}");
        }

        public WiqlDirectLinksQueryBuilder AddSourceCondition(string conjunction, string parameter, string comparisonOperator, int? value)
            => AddSourceCondition($"{conjunction} [Source].{parameter} {comparisonOperator} {value}");

        public WiqlDirectLinksQueryBuilder AddSourceCondition(string conjunction, string parameter, string comparisonOperator, IEnumerable<int> values)
            => AddSourceCondition($"{conjunction} [Source].{parameter} {comparisonOperator} ({String.Join(",", values)})");

        public WiqlDirectLinksQueryBuilder AddSourceCondition(string condition)
        {
            query.SourceConditions.Add(condition);

            return this;
        }

        #endregion source
    }
}
