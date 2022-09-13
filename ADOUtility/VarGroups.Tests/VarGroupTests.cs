using ADOCore;
using ADOCore.ApiClietns;
using ADOCore.Models.VariableGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using System.Collections.Generic;
using System.IO;
using VarGroups.Tests.Ulitlities;

namespace VarGroups.Tests
{
    [TestClass]
    public class VarGroupTests
    {
        [TestInitialize]
        public void TestInit()
        {
            var adoSettingsReader = new SettingsReader("ADOconfig.json");
            adoSettings = new AdoSettings(adoSettingsReader);
            apiClient = new VariableGroupsApiClient(adoSettings);

        }

        private AdoSettings adoSettings;
        private VariableGroupsApiClient apiClient;

        [TestMethod]
        public void GetVarGroups()
        {
            var envNames = new List<string>() 
            {
                "Highlander","Testauto1","Testauto3","UkIntegration","UkNewInf","UKQA","UKQA1","UsBrest2","UsIntegration","UsMilkyWay","UsNewInf","UsPlatform","UsPlatform2sa","USQA","USQA1","UsRacoons2"
            };

            List<VariableGroup> varGroups = new();
            foreach (var envName in envNames)
            {
                var varGroupName = VarGroupNamesGenerator.GenerateVarGroupName(envName);
                var varGroup = apiClient.GetVariableGroup(varGroupName);
                varGroups.Add(varGroup);
               
            }

            var equalVariables = varGroups.GetEqualValuesKeysValue();
            var equalKeys = equalVariables.GetAllKeys();
            var varsToInclude = varGroups.GetAllKeys();

            foreach (var varGroup in varGroups)
            {
                var yaml = varGroup.ToYamlStr(equalKeys, varsToInclude, out string envHost);
                var path = $"C:\\Users\\Aksana_Murashka\\Documents\\TRI-SRTR\\Pipelines\\{envHost}.yml";
                File.WriteAllText(path, yaml);
            }

            var commonyaml = equalVariables.ToYamlStr();
            var commonpath = $"C:\\Users\\Aksana_Murashka\\Documents\\TRI-SRTR\\Pipelines\\common-env-data.yml";
            File.WriteAllText(commonpath, commonyaml);
        }

    }
}