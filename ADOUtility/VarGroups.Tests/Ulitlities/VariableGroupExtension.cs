using ADOCore.Models.VariableGroups;
using System.Linq;
using System.Text;

namespace VarGroups.Tests.Ulitlities
{
    internal static class VariableGroupExtension
    {
        internal static string ToYamlStr(this VariableGroup variableGroup)
        {
            var variables = variableGroup.value.First().variables;
            var yamlStr = new StringBuilder();
            yamlStr.AppendLine("variables:");
            foreach (var variable in variables)
            {
                yamlStr.AppendLine($"  {variable.Key}: '{variable.Value.Value}'");
            }

            return yamlStr.ToString();
        }
    }
}
