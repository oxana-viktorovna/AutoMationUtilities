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
            var responce = SendAdoRequest("wit/wiql", Method.Post, body: new WiqlQueryRequest(query));
            var content = JsonConvert.DeserializeObject<WorkItemResponce>(responce.Content);

            return content;
        }
        
    }
}
