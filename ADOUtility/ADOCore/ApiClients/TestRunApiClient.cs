using ADOCore.ApiClients;
using ADOCore.Models;
using RestSharp;
using SharedCore.FileUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ADOCore.ApiClietns
{
    public class TestRunApiClient : CoreAdoApiClient
    {
        public TestRunApiClient(AdoSettings adoSettings) : base(adoSettings)
        {
        }

        public List<RunInfo> GetRunInfo(IEnumerable<int> runIds)
            => runIds.Select(runId => GetRunInfo(runId)).ToList();

        public RunInfo GetRunInfo(int runId)
        {
            var response = SendAdoRequest($"test/Runs/{runId}", Method.Get);
            var runsInfo = JsonSerializer.Deserialize<RunInfo>(response.Content);

            return runsInfo;
        }

        public RunStat GetRunStatistic(int runId)
        {
            var response = SendAdoRequest($"test/Runs/{runId}/Statistics", Method.Get);

            if (response.Content.Contains("<!DOCTYPE html>"))
                return null;

            var runStat = JsonSerializer.Deserialize<RunStat>(response.Content);

            return runStat;
        }

        public List<RunStat> GetRunStatistic(IEnumerable<int> runIds)
            => runIds.Select(runId => GetRunStatistic(runId)).ToList();

        public AttachmentsInfo GetAttachmentsInfo(int runId)
        {
            if (runId == 0)
                return null;

            var response = SendAdoRequest($"test/Runs/{runId}/attachments", Method.Get);

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

        private RestResponse GetAttachement(int runId, int attachmentId)
            => attachmentId == 0 
            ? null 
            : SendAdoRequest($"test/Runs/{runId}/attachments/{attachmentId}", Method.Get);
    }
}
