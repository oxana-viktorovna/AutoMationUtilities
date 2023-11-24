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

        public List<RunInfo> GetRunInfo(IEnumerable<int> runIds)
            => runIds.Select(runId => GetRunInfo(runId)).ToList();

        public RunInfo GetRunInfo(int runId)
        {
            var response = SendAdoRequest($"test/Runs/{runId}", Method.GET);
            var runsInfo = JsonSerializer.Deserialize<RunInfo>(response.Content);

            return runsInfo;
        }

        public T GetTestPlanDetails<T>(int planId, int? suitId)
        {
            string apiUrl;
            Type responseType;
            string response;
            if (suitId != 0)
            { 
                apiUrl = $"testplan/Plans/{planId}/suites/{suitId}?expand=Children&api-version=7.2-preview.1";
            }
            else
            {
                apiUrl = $"testplan/Plans/{planId}/suites?api-version=7.2-preview.1";
            }
            response = SendAdoRequest(apiUrl, Method.GET).Content;
            return JsonSerializer.Deserialize<T>(response);
        }
        public TestCaseResponse GetTestIds(int planId, int suitId)
        {
            var response = SendAdoRequest($"testplan/Plans/{planId}/Suites/{suitId}/TestCase", Method.GET);
            var testPlanInfo = JsonSerializer.Deserialize<TestCaseResponse>(response.Content);
            return testPlanInfo;
        }

        public RunStat GetRunStatistic(int runId)
        {
            var response = SendAdoRequest($"test/Runs/{runId}/Statistics", Method.GET);

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
