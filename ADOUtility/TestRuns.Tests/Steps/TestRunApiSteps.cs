using ADOCore;
using ADOCore.ApiClients;
using ADOCore.ApiClietns;
using ADOCore.Models;
using System.Collections.Generic;
using System.Linq;
using TestRuns.Utilities;

namespace TestRuns.Steps
{
    public class TestRunApiSteps
    {
        public TestRunApiSteps(AdoSettings adoSettings)
        {
            testRunApiClient = new TestRunApiClient(adoSettings);
            testResultDetailApiClient = new ResultDetailsByBuildApiClient(adoSettings);
            buildsApiClient = new BuildsApiClient(adoSettings);
        }

        private readonly TestRunApiClient testRunApiClient;
        private readonly ResultDetailsByBuildApiClient testResultDetailApiClient;
        private readonly BuildsApiClient buildsApiClient;

        //public List<TestRun> GetTrxAttachements(string runTitle)
        //{
        //    var runDate = RunTitleParser.GetDataFromTitle(runTitle);
        //    var runIds = testRunApiClient.GetRunIds(runTitle, runDate);
        //    var runAllAttachIds = runIds.Select(runId => (runId, testRunApiClient.GetAttachementIds(runId)));
        //    var lastRunAttachIds = runAllAttachIds.Select(runAttachId => (runAttachId.runId, runAttachId.Item2.Last()));
        //    var trxAttachements = lastRunAttachIds.Select(raId => testRunApiClient.GetTrxResults(raId.runId, raId.Item2)).ToList();

        //    return trxAttachements;
        //}

        public string GetBuildNumber(int buildId)
            => buildId == 0 
            ? null 
            : buildsApiClient.GetBuild(buildId).buildNumber;

        public List<RunStat> GetRunStatistics(int buildId)
        {
            var runIds = testResultDetailApiClient.GetRunIds(buildId);

            return testRunApiClient.GetRunStatistic(runIds);
        }

        public List<TestRun> GetTrxAttachements(int buildId)
        {
            var allAtchsInfos = GetAllBuildAttchmentsInfo(buildId);
            var trxAttchIds = GetLastAttchsIdsByType(allAtchsInfos, ".trx");
            var trxAttachements = GetTxrAttachments(trxAttchIds);

            return trxAttachements;
        }

        public List<TestRun> GetTrxAttachements(int buildId, int excludeRunId = 0)
        {
            var allAtchsInfos = GetAllBuildAttchmentsInfo(buildId);
            var atchsInfos = excludeRunId == 0
                ? allAtchsInfos
                : allAtchsInfos.Where(r => r.runId != excludeRunId);

            var trxAttchIds = GetLastAttchsIdsByType(atchsInfos, ".trx");
            var trxAttachements = GetTxrAttachments(trxAttchIds);

            return trxAttachements;
        }

        public List<TestRun> GetTrxAttachements(List<int> runIds)
        {
            var atchsInfos = GetAttchmentsInfo(runIds);

            var trxAttchIds = GetLastAttchsIdsByType(atchsInfos, ".trx");
            var trxAttachements = GetTxrAttachments(trxAttchIds);

            return trxAttachements;
        }

        public List<TestRun> GetTrxAttachementsSignleRun(int buildId, int runId)
        {
            var allAtchsInfos = GetAllBuildAttchmentsInfo(buildId);
            var allAtchsInfo = allAtchsInfos.Where(r => r.runId == runId);

            var trxAttchIds = GetLastAttchsIdsByType(allAtchsInfo, ".trx");
            var trxAttachements = GetTxrAttachments(trxAttchIds);

            return trxAttachements;
        }

        public List<testsuites> GetJUnitAttachements(int buildId)
        {
            var allAtchsInfos = GetAllBuildAttchmentsInfo(buildId);
            var trxAttchIds = GetLastAttchsIdsByType(allAtchsInfos, ".xml");
            var trxAttachements = GetJUnitAttachments(trxAttchIds);

            return trxAttachements;
        }

        private List<(int runId, int attchId)> GetLastAttchsIdsByType(IEnumerable<(int runId, AttachmentsInfo attchInfos)> runAttchInfos, string type)
        {
            var ids = new List<(int runId, int attchId)>();
            foreach (var (runId, attchInfos) in runAttchInfos)
            {
                var lastAttch = attchInfos.value.LastOrDefault();
                if (lastAttch == null)
                    continue;
                if (lastAttch.fileName.Contains(type))
                    ids.Add((runId, lastAttch.id));
            }

            return ids;
        }

        private List<TestRun> GetTxrAttachments(IEnumerable<(int runId, int attchId)> ids)
            => ids.Select(raId => testRunApiClient.GetTrxResults(raId.runId, raId.attchId)).ToList();

        private List<testsuites> GetJUnitAttachments(IEnumerable<(int runId, int attchId)> ids)
            => ids.Select(raId => testRunApiClient.GetJUnitReport(raId.runId, raId.attchId)).ToList();

        private List<(int runId, AttachmentsInfo attchInfos)> GetAllBuildAttchmentsInfo(int buildId)
        {
            var runIds = testResultDetailApiClient.GetRunIds(buildId);
            
            return runIds.Select(runId => (runId, testRunApiClient.GetAttachmentsInfo(runId))).ToList();
        }

        private List<(int runId, AttachmentsInfo attchInfos)> GetAttchmentsInfo(List<int> runIds)
        {
            return runIds.Select(runId => (runId, testRunApiClient.GetAttachmentsInfo(runId))).ToList();
        }
    }
}
