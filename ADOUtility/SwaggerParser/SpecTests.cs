using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedCore.Settings;
using SwaggerParser;
using SwaggerParser.Models;
using SwaggerParser.Utilities;
using System.Collections.Generic;
using System.IO;

namespace APISchema
{
    [TestClass]
    public class UnitTest1
    {
        private SwaggerParserTestSettings testSettings;

        [TestInitialize]
        public void TestInit()
        {
            var testSettingsReader = new SettingsReader("SwaggerParserConfig.json");
            testSettings = new SwaggerParserTestSettings(testSettingsReader);
        }

        [TestMethod]
        public void GetDevPortalSwaggerSpec()
        {
            var apiNames = new List<string>
            {
            "UserService.Api_.v1.External.DevPortal",
            "UserMapping.Api_.v1.External.DevPortal",
            "reports-api.v2",
            "UserInfo.Api_.v1.External.DevPortal",
            "matters-api.v1",
            "locations-api.v1",
            "firms-api.v2",
            "EFiles.Api_.v1.External.DevPortal",
            "DMS.Api_.v1.External.DevPortal",
            "ap-data-exchange-api",
            "company-list-api",
            "datawarehouse-api.v1",
            "company-api.v1",
            "billing-api.v1",
            "admin-api.v1"
            };

            var apistats = new List<ApiStat>();
            var swagerParser = new SpecParser();
            foreach (var apiName in apiNames)
            {
                dynamic json = swagerParser.ReadDynamicJson(testSettings.SpecFolder, apiName);
                apistats.AddRange(swagerParser.SwaggerJsonToApiStat(apiName, json));
            }


            var savePath = Path.Combine(testSettings.SpecFolder, "DevPortal.csv");
            File.WriteAllText(savePath, apistats.ApiStatsToCvsString());
        }

        [TestMethod]
        public void GetInternalSwaggerSpec()
        {
            var apiNames = new List<string>
            {
                "InternalV1","InternalV2"
            };

            var apistats = new List<ApiStat>();
            var swagerParser = new SpecParser();
            foreach (var apiName in apiNames)
            {
                dynamic json = swagerParser.ReadDynamicJson(testSettings.SpecFolder, apiName);
                apistats.AddRange(swagerParser.SwaggerJsonToApiStat(apiName, json));
            }


            var savePath = Path.Combine(testSettings.SpecFolder, "Internal.csv");
            File.WriteAllText(savePath, apistats.ApiStatsToCvsString());
        }

        [TestMethod]
        public void GetPostmanCollection()
        {           
            var collections = new List<string>
            {
            "InvoicesV1Tests",
            "InvoicesV2Tests"
            };
            var apistats = new List<ApiStat>();

            foreach (var collection in collections)
            {
                var parser = new PostmanCollectionParser(collection);
                apistats.AddRange(parser.ReadCollectionAsApiStat(testSettings.CollFolder));
            }

            var savePath = Path.Combine(testSettings.CollFolder, "LegalTrackerServices.csv");
            File.WriteAllText(savePath, apistats.ApiStatsToCvsString());
        }
    }
}