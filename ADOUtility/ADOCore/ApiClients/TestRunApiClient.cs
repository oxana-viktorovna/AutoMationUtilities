using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ADOCore.Models;
using ADOCore.ApiClients;
using SharedCore.FileUtilities;

namespace ADOCore.ApiClietns
{
    public class TestRunApiClient : CoreAdoApiClient
    {
        public TestRunApiClient(AdoSettings adoSettings) : base(adoSettings)
        {
        }

        public List<int> GetRunIds(string runTitle, DateTime runDate)
        {
            var minLastUpdatedDate = runDate.ToString("yyyy-MM-dd");
            var maxLastUpdatedDate = (runDate.AddDays(2)).ToString("yyyy-MM-dd");
            var parameters = new List<(string, string)>
            {
                ("runTitle", runTitle),
                ("minLastUpdatedDate", minLastUpdatedDate),
                ("maxLastUpdatedDate", maxLastUpdatedDate)
            };

            var response = SendAdoRequest("test/Runs", Method.GET, parameters);
            var runsInfo = JsonSerializer.Deserialize<RunInfoResponce>(response.Content);
            var ids = runsInfo.value.Select(v => v.id);

            return ids.ToList();
        }

        public RunStat GetRunStatistic(int runId)
        {
            var response = SendAdoRequest($"test/Runs/{runId}/Statistics", Method.GET);
            var runStat = JsonSerializer.Deserialize<RunStat>(response.Content);

            return runStat;
        }

        public List<RunStat> GetRunStatistic(IEnumerable<int> runIds)
            => runIds.Select(runId => GetRunStatistic(runId)).ToList();

        public AttachmentsInfo GetAttachmentsInfo(int runId)
        {
            if (runId == 0)
                return null;

            var response = SendAdoRequest($"test/Runs/{runId}/attachments", Method.GET);

            return JsonSerializer.Deserialize<AttachmentsInfo>(response.Content);
        }

        public TestRun GetTrxResults(int runId, int attachmentId)
        {
            var response = GetAttachement(runId, attachmentId);

            return XmlWorker.DeserializeXmlFromMemoryStream<TestRun>(response.Content);
        }

        public testsuites GetJUnitReport(int runId, int attachmentId)
        {
            var response = GetAttachement(runId, attachmentId);

            return XmlWorker.DeserializeXmlFromMemoryStream<testsuites>(response.Content);
        }

        private IRestResponse GetAttachement(int runId, int attachmentId)
            => attachmentId == 0 
            ? null 
            : SendAdoRequest($"test/Runs/{runId}/attachments/{attachmentId}", Method.GET);
    }
}
