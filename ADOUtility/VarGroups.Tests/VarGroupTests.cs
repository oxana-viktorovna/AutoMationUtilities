using ADOCore;
using ADOCore.ApiClietns;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
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
        public void GetVarGroup()
        {
            var envName = "Testauto1";
            var varGroupName = VarGroupNamesGenerator.GenerateVarGroupName(envName);
            var varGroup = apiClient.GetVariableGroup(varGroupName);
            var yaml = varGroup.ToYamlStr();
            var path = $"C:\\Users\\Aksana_Murashka\\Documents\\TRI-SRTR\\Pipelines\\{envName}.yml";
            File.WriteAllText(path, yaml);
        }
    }
}