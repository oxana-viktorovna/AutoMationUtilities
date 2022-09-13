using ADOCore.Models.VariableGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VarGroups.Tests.Ulitlities
{
    internal static class VariableGroupExtension
    {
        internal static string ToYamlStr(this VariableGroup variableGroup, List<string> varToExclued, List<string> varToInclued, out string envHost)
        {
            var variables = variableGroup.value.First().variables;
            var emptyValue = new VariableValue("")
            {
                Value = ""
            };

            foreach (var variable in varToInclued)
            {
                if (!variable.Contains(variable) && !varToExclued.Contains(variable))
                    variables.Add(variable, emptyValue);
            }
            var variablescleaned = variables.Where(v => !varToExclued.Contains(v.Key)).ToDictionary(p => p.Key, p => p.Value);

            var yamlStr = new StringBuilder();
            yamlStr.AppendLine("variables:");

            AddGroupedVariables(variablescleaned, "General", new List<string>() { "TestEnvironment", "TrackerBuildNameHotfix", "EnvironmentXML" }, ref yamlStr);
            AddGroupedVariables(variablescleaned, "Urls", new List<string>() { "TestUrl", "GatewayUrl", "HostName" }, ref yamlStr);
            AddGroupedVariables(variablescleaned, "Users", "User", ref yamlStr);
            AddGroupedVariables(variablescleaned, "Machines", new List<string>() { "Svc", "SQL", "Web", "Sub" }, ref yamlStr);
            AddGroupedVariables(variablescleaned, "Arm", "Arm", ref yamlStr);
            AddGroupedVariables(variablescleaned, "Azure", new List<string>() { "Azure", "CustomValues", "CustomKeys", "GatewayDeployKeyVaultName", "ServiceBusKey", "ResourceGroup" }, ref yamlStr);

            var envHostKeyVal = variables.Where(v => v.Key.Equals("EnvHostName"));
            envHost = envHostKeyVal.Any()
                ? envHostKeyVal.First().Value.Value
                : "";

            return yamlStr.ToString();
        }

        internal static string ToYamlStr(this VariableGroup variableGroup)
        {
            var variables = variableGroup.value.First().variables;
            var variablesSorted = new SortedDictionary<string, VariableValue>(variables);

            var yamlStr = new StringBuilder();
            yamlStr.AppendLine("variables:");

            foreach (var variable in variablesSorted)
            {
                yamlStr.AppendLine($"  {variable.Key}: '{variable.Value.Value}'");
            }

            return yamlStr.ToString();
        }

        private static void AddGroupedVariables(
            Dictionary<string, VariableValue> variables,
            string groupTitle,
            string condition,
            ref StringBuilder yamlStr)
        {
            var variablesSorted = new SortedDictionary<string, VariableValue>(variables);
            var groupedVars = variablesSorted.Where(v => v.Key.Contains(condition));

            yamlStr.AppendLine($"#{groupTitle}");
            foreach (var variable in groupedVars)
            {
                yamlStr.AppendLine($"  {variable.Key}: '{variable.Value.Value}'");
            }

        }

        private static void AddGroupedVariables(
            Dictionary<string, VariableValue> incomeVariables,
            string groupTitle,
            List<string> conditions,
            ref StringBuilder yamlStr)
        {
            yamlStr.AppendLine($"#{groupTitle}");

            SortedDictionary<string, VariableValue> variables = new();
            conditions = conditions.OrderBy(c => c).ToList();
            foreach (var condition in conditions)
            {
                var groupedVars = incomeVariables.Where(v => v.Key.Contains(condition)).ToDictionary(p => p.Key, p => p.Value);
                variables.AddRange(groupedVars);
            }

            foreach (var variable in variables)
            {
                yamlStr.AppendLine($"  {variable.Key}: '{variable.Value.Value}'");
            }
        }

        public static VariableGroup GetEqualValuesKeysValue(this List<VariableGroup> varGroups)
        {
            Dictionary<string, VariableValue> equalKeys = new();

            var environments = varGroups.Select(vg => vg.value.First().variables).ToList();
            var allKeys = environments.SelectMany(env => env.Keys).Distinct();

            foreach (var key in allKeys)
            {
                bool areAllEqual = true;
                bool isFormula = false;
                VariableValue value = null;
                for (int i = 0; i < environments.Count() - 1; i++)
                {
                    if (environments[i].TryGetValue(key, out VariableValue currValue)
                        && environments[i + 1].TryGetValue(key, out VariableValue nextValue))
                    {
                        if (string.IsNullOrEmpty(currValue.Value) || string.IsNullOrEmpty(nextValue.Value))
                            continue;

                        if (currValue.Value != nextValue.Value)
                        {
                            areAllEqual = false;
                            break;
                        }

                        if (currValue.Value.Contains('$'))
                            isFormula = true;

                        value = nextValue;
                    }
                }
                if (areAllEqual && !isFormula)
                {
                    if (value == null)
                        value = new VariableValue("");
                    if (value.Value == null)
                        value.Value = "";
                    equalKeys.Add(key, value);
                }
            }

            VariableGroup varGroup = new();
            varGroup.value = new Value[1];
            var val = new Value();
            val.variables = equalKeys;
            varGroup.value[0] = val;

            return varGroup;
        }

        public static List<string> GetAllKeys(this List<VariableGroup> varGroups)
        {
            var environments = varGroups.Select(vg => vg.value.First().variables).ToList();
            var allKeys = environments.SelectMany(env => env.Keys).Distinct().ToList();

            return allKeys;
        }

        public static List<string> GetAllKeys(this VariableGroup varGroup)
        {
            var allKeys = varGroup.value.First().variables.Keys.Distinct().ToList();

            return allKeys;
        }

        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            foreach (var element in source)
                target.Add(element);
        }
    }
}
