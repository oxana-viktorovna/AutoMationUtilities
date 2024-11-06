using RestSharp;
using ADOCore.ApiClients;
using ADOCore.Models;
using Newtonsoft.Json;

namespace ADOCore.ApiClietns
{
    public class WiqlApiClient : CoreAdoApiClient
    {
        public WiqlApiClient(AdoSettings adoSettings) : base(adoSettings)
        {
        }

        public WorkItemResponce PostWiqlQuery(string query)
        { 
            var responce = SendAdoRequest("wit/wiql", Method.POST, body: new WiqlQueryRequest(query));
            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
                throw new System.Exception(responce.Content);

            return JsonConvert.DeserializeObject<WorkItemResponce>(responce.Content);
        }
        
    }
}
