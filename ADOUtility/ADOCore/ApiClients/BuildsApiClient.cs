using ADOCore.Models;
using RestSharp;
using System.Text.Json;

namespace ADOCore.ApiClients
{
    public class BuildsApiClient : CoreAdoApiClient
    {
        public BuildsApiClient(AdoSettings adoSettings) : base(adoSettings)
        {
        }

        public GetBuildResponce GetBuild(int buildId)
        {
            var response = SendAdoRequest($"build/builds/{buildId}", Method.GET);

            return JsonSerializer.Deserialize<GetBuildResponce>(response.Content);
        }
    }
}
