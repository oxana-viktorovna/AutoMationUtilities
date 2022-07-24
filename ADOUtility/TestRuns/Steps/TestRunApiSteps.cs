using ADOCore;
using ADOCore.ApiClients;
using ADOCore.ApiClietns;
using ADOCore.Models;
using System.Text.RegularExpressions;
using TestRuns.Models;
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

        public string GetBuildNumber(int buildId)
            => buildId == 0
                ? string.Empty
                : buildsApiClient.GetBuild(buildId).buildNumber;

        public List<RunStat> GetRunStatistics(int buildId)
        {
            var runIds = testResultDetailApiClient.GetRunIds(buildId);

            return testRunApiClient.GetRunStatistic(runIds);
        }

        public List<TestRun> GetTrxAttachements(int buildId, Outcome outcome)
        {
            var allAtchsInfos = GetAllBuildAttchmentsInfo(buildId);

            return outcome switch
            {
                Outcome.Passed => GetAttachementsPassed(allAtchsInfos),
                _ => GetAttachementsFailed(allAtchsInfos),
            };
        }

        public List<TestRun> GetTrxAttachementsExcludeRun(int buildId, string runNamePatternToExclude, Outcome outcome)
        {
            var allAtchsInfos = GetBuildAttchmentsInfo(buildId, runNamePatternToExclude, true);

            return outcome switch
            {
                Outcome.Passed => GetAttachementsPassed(allAtchsInfos),
                _ => GetAttachementsFailed(allAtchsInfos),
            };
        }

        public List<TestRun> GetTrxAttachementsExcludeRun(int buildId, List<int> rerunsBuildIds, string runNamePatternToExclude, Outcome outcome)
        {
            var allRuns = new List<int>() { buildId };
            if (rerunsBuildIds is not null)
                allRuns.AddRange(rerunsBuildIds);

            var reRunUiTestResults = new List<TestRun>();

            foreach (var run in allRuns)
            {
                reRunUiTestResults.AddRange(GetTrxAttachementsExcludeRun(run, runNamePatternToExclude, outcome));
            }

            return reRunUiTestResults;
        }

        public List<TestRun> GetTrxAttachementsSignleRun(int buildId, string runNamePatternToInclude, Outcome outcome)
        {
            var allAtchsInfos = GetBuildAttchmentsInfo(buildId, runNamePatternToInclude, false);

            return outcome switch
            {
                Outcome.Passed => GetAttachementsPassed(allAtchsInfos),
                _ => GetAttachementsFailed(allAtchsInfos),
            };
        }

        public List<TestRun> GetTrxAttachements(List<int> runIds)
        {
            var allAtchsInfos = GetAttchmentsInfo(runIds);

            return GetAttachementsFailed(allAtchsInfos);
        }

        public List<testsuites> GetJUnitAttachements(int buildId)
        {
            var allAtchsInfos = GetAllBuildAttchmentsInfo(buildId);
            var trxAttchIds = GetLastAttchsIdsByType(allAtchsInfos, ".xml");
            var trxAttachements = GetJUnitAttachments(trxAttchIds);

            return trxAttachements;
        }

        public List<testsuites> GetJUnitAttachements(int buildId, List<int> rerunsBuildIds)
        {
            var apiTestResults = GetJUnitAttachements(buildId);

            if (!apiTestResults.Any() && rerunsBuildIds is not null)
            {
                for (int i = 0; i < rerunsBuildIds.Count; i++)
                {
                    if (!apiTestResults.Any())
                    {
                        apiTestResults = GetJUnitAttachements(rerunsBuildIds[i]);
                    }
                }
            }

            return apiTestResults;
        }

        public List<ResultReport> CopyCommentsForBlocked(int buildId, string blockedPattern, string saveFolder)
        {
            var blockedTestResults = GetTrxAttachementsSignleRun(buildId, blockedPattern, Outcome.Failed);
            var uiFailedBlockedTests = ResultReportConverter.Convert(blockedTestResults.GetFailedResults());
            var uiFailedBlockedTestsWithComments = new ReportFileSteps().CompareResultsWithBlockers(saveFolder, "Blocked", uiFailedBlockedTests);

            return uiFailedBlockedTestsWithComments;
        }

        private static List<(int runId, int attchId)> GetLastAttchsIdsByType(IEnumerable<(int runId, AttachmentsInfo attchInfos)> runAttchInfos, string type)
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

        private static List<(int runId, int attchId)> GetAttchsIdsByType(IEnumerable<(int runId, AttachmentsInfo attchInfos)> runAttchInfos, string type)
        {
            var ids = new List<(int runId, int attchId)>();
            foreach (var (runId, attchInfos) in runAttchInfos)
            {
                foreach (var attchInfo in attchInfos.value)
                {
                    if (attchInfo.fileName.Contains(type))
                        ids.Add((runId, attchInfo.id));
                }
            }

            return ids;
        }

        private List<TestRun> GetTxrAttachments(IEnumerable<(int runId, int attchId)> ids)
            => ids.Select(raId => testRunApiClient.GetTrxResults(raId.runId, raId.attchId)).ToList();

        private List<testsuites> GetJUnitAttachments(IEnumerable<(int runId, int attchId)> ids)
            => ids.Select(raId => testRunApiClient.GetJUnitReport(raId.runId, raId.attchId)).ToList();

        private List<(int runId, AttachmentsInfo attchInfos)> GetBuildAttchmentsInfo(
            int buildId,
            string runNamePattern,
            bool toExclude = false)
        {
            var regex = new Regex(runNamePattern);
            var runIds = testResultDetailApiClient.GetRunIds(buildId);
            var runInfos = testRunApiClient.GetRunInfo(runIds);
            var selectedRunInfos = toExclude
                ? runInfos.Where(runInfo => !regex.IsMatch(runInfo.name))
                : runInfos.Where(runInfo => regex.IsMatch(runInfo.name));
            var selectedRunIds = selectedRunInfos.Select(runInfo => runInfo.id).ToList();

            return selectedRunIds.Select(runId => (runId, testRunApiClient.GetAttachmentsInfo(runId))).ToList();
        }

        private List<(int runId, AttachmentsInfo attchInfos)> GetAllBuildAttchmentsInfo(int buildId)
        {
            var runIds = testResultDetailApiClient.GetRunIds(buildId);

            return runIds.Select(runId => (runId, testRunApiClient.GetAttachmentsInfo(runId))).ToList();
        }

        private List<(int runId, AttachmentsInfo attchInfos)> GetAttchmentsInfo(List<int> runIds)
        {
            return runIds.Select(runId => (runId, testRunApiClient.GetAttachmentsInfo(runId))).ToList();
        }

        private List<TestRun> GetAttachementsFailed(IEnumerable<(int runId, AttachmentsInfo attchInfos)> allAtchsInfos)
        {
            var trxAttchIds = GetLastAttchsIdsByType(allAtchsInfos, ".trx");

            return GetTxrAttachments(trxAttchIds);
        }

        private List<TestRun> GetAttachementsPassed(IEnumerable<(int runId, AttachmentsInfo attchInfos)> allAtchsInfos)
        {
            var trxAttchIds = GetAttchsIdsByType(allAtchsInfos, ".trx");

            return GetTxrAttachments(trxAttchIds);
        }
    }
}
