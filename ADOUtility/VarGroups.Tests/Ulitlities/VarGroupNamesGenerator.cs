using System.Collections.Generic;
using System.Linq;

namespace VarGroups.Tests.Ulitlities
{
    internal static class VarGroupNamesGenerator
    {
        internal static string GenerateVarGroupName(string envName)
            => $"WIP-{envName}-EnvData";

        internal static List<string> GenerateVarGroupName(IEnumerable<string> envNames)
            => envNames.Select(envName => GenerateVarGroupName(envName)).ToList();
    }
}
