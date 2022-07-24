using ADOCore.ApiClients;
using ADOCore.Models.VariableGroups;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace ADOCore.ApiClietns
{
    public class VariableGroupsApiClient : CoreAdoApiClient
    {
        public VariableGroupsApiClient(AdoSettings adoSettings) : base(adoSettings)
        {
        }

        public VariableGroup GetVariableGroup(string varGroupName)
        {
            var paramseters = new List<(string name, string value)>
            {
                ("groupName", varGroupName)
            };
            var response = SendAdoRequest("distributedtask/variablegroups", Method.GET, paramseters, "6.0-preview.2");
            var varGroup = JsonConvert.DeserializeObject<VariableGroup>(response.Content);

            return varGroup;
        }
    }
}
