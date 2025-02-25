using ADOCore.ApiClietns;
using ADOCore.Models;
using System.Linq;

namespace ADOCore.Steps
{
    public class WiqlQuerySteps
    {
        public WiqlQuerySteps(AdoSettings adoSettings)
        {
            wiqlApiClient = new WiqlApiClient(adoSettings);
        }

        private readonly WiqlApiClient wiqlApiClient;

        public Workitemrelation[] GetLinkedItems(string? query)
        {
            var response = wiqlApiClient.PostWiqlQueryLinkedItems(query);

            return response.workItemRelations.Where(w => w.rel != null).ToArray();
        }

    }
}
