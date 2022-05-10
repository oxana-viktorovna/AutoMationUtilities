using System;
using System.Collections.Generic;
using System.Text;

namespace ADOCore.Models.VariableGroups
{
    public class VariableGroup
    {
        public string Description { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public List<(string, VariableValue)> Variables { get; set; }

        //VariableGroupProviderData providerData
        //VariableGroupProjectReference[] variableGroupProjectReferences
    }
}
