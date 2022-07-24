using ADOCore.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ADOCore.ApiClients
{
    public class ResultDetailsByBuildApiClient : CoreAdoApiClient
    {
        public ResultDetailsByBuildApiClient(AdoSettings adoSettings) : base(adoSettings)
        {
        }

        public List<int> GetRunIds(int buildId)
        {
            var parameters = new List<(string, string)>
            {
                ("buildId", buildId.ToString())
            };

            var response = SendAdoRequest("test/ResultDetailsByBuild", Method.GET, parameters, version: "7.0-preview");
            var resultDetails = JsonSerializer.Deserialize<ResultDetailsByBuildResponce>(response.Content);
            var allids = resultDetails.resultsForGroup.SelectMany(rg => rg.results.Select(r => Convert.ToInt32(r.testRun.id)));
            var ids = allids.Distinct().ToList();

            return ids;
        }
    }
}
