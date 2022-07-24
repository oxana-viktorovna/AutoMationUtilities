using System;
using System.Collections.Generic;

namespace ADOCore.Models.VariableGroups
{
    public class VariableGroup
    {
        public int count { get; set; }
        public Value[] value { get; set; }
    }

    public class Value
    {
        public Dictionary<string, VariableValue> variables { get; set; }
        public int id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Createdby createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public Modifiedby modifiedBy { get; set; }
        public DateTime modifiedOn { get; set; }
        public bool isShared { get; set; }
        public object variableGroupProjectReferences { get; set; }
    }

    public class Createdby
    {
        public string displayName { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
    }

    public class Modifiedby
    {
        public string displayName { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
    }
}
