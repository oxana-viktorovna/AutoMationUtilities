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

        public List<int> GetAllBuildsIds(int buildId, List<int> rerunsBuildIds)
        {
            var allRuns = new List<int>() { buildId };
            if (rerunsBuildIds is not null)
                allRuns.AddRange(rerunsBuildIds);

            return allRuns;
        }

        public List<RunStat> GetRunStatistics(int buildId)
        {
            var runIds = testResultDetailApiClient.GetRunIds(buildId);

            return testRunApiClient.GetRunStatistic(runIds);
        }

        #region Trx

        public IEnumerable<TestRunUnitTestResult> GetAllTrxRunResults(int buildId)
        {
            var allTestRuns = GetTrxAttachements(buildId);
            var allTestResults = allTestRuns.GetAllRunResults();

            return allTestResults;
        }

        public IEnumerable<TestRunUnitTestResult> GetAllTrxRunResultsExcludeRun(int buildId, string runNamePatternToExclude)
        {
            var allTestRuns = GetTrxAttachementsExcludeRun(buildId, runNamePatternToExclude);
            var allTestResults = allTestRuns.GetAllRunResults();

            return allTestResults;
        }

        public IEnumerable<TestRunUnitTestResult> GetAllTrxRunResultsExcludeRun(List<int> allBuildIds, string runNamePatternToExclude)
        {
            var allTestRuns = GetTrxAttachementsExcludeRun(allBuildIds, runNamePatternToExclude);
            var allTestResults = allTestRuns.GetAllRunResults();

            return allTestResults;
        }

        public IEnumerable<TestRunUnitTestResult> GetAllTrxRunResultsIncludeRun(int buildId, string runNamePatternToInclude)
        {
            var allTestRuns = GetTrxAttachementsIncludeRun(buildId, runNamePatternToInclude);
            var allTestResults = allTestRuns.GetAllRunResults();

            return allTestResults;
        }

        public List<TestRun> GetTrxAttachements(int buildId)
        {
            var allAtchsInfos = GetAttchmentsInfoByBuildId(buildId);

            return GetTxrAttachments(allAtchsInfos);
        }

        public List<TestRun> GetTrxAttachements(List<int> runIds)
        {
            var allAtchsInfos = GetAttchmentsInfoByRunIds(runIds);

            return GetTxrAttachments(allAtchsInfos);
        }

        public List<TestRun> GetTrxAttachementsExcludeRun(List<int> allBuildIds, string runNamePatternToExclude)
            => allBuildIds.SelectMany(buildId => GetTrxAttachementsExcludeRun(buildId, runNamePatternToExclude)).ToList();

        public List<TestRun> GetTrxAttachementsExcludeRun(int buildId, string runNamePatternToExclude)
        {
            var allAtchsInfos = GetAttchmentsInfoByBuildId(buildId, runNamePatternToExclude, true);

            return GetTxrAttachments(allAtchsInfos);
        }

        public List<TestRun> GetTrxAttachementsIncludeRun(int buildId, string runNamePatternToInclude)
        {
            var allAtchsInfos = GetAttchmentsInfoByBuildId(buildId, runNamePatternToInclude, false);

            return GetTxrAttachments(allAtchsInfos);
        }

        public List<ResultReport> CopyCommentsForBlocked(IEnumerable<TestRunUnitTestResult> blockedTests, string saveFolder)
        {
            if (!blockedTests.Any())
                return null;

            var uiFailedBlockedTests = ResultReportConverter.Convert(blockedTests.GetFailedResults());
            var uiFailedBlockedTestsWithComments = new ReportFileSteps().CompareResultsWithBlockers(saveFolder, "Blockers", uiFailedBlockedTests);

            return uiFailedBlockedTestsWithComments;
        }

        private List<TestRun> GetTxrAttachments(IEnumerable<(int runId, int attchId)> ids)
            => ids.Select(raId => testRunApiClient.GetTrxResults(raId.runId, raId.attchId)).ToList();

        private List<TestRun> GetTxrAttachments(List<(int runId, AttachmentsInfo attchInfos)> attchmentsInfos)
        {
            var trxAttchIds = GetAttchsIdsByType(attchmentsInfos, ".trx");

            return GetTxrAttachments(trxAttchIds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildId"></param>
        /// <param name="runNamePattern"></param>
        /// <param name="toExclude">false will include only runs by pattern. true will exclude runs by pattern.</param>
        /// <returns></returns>
        private List<(int runId, AttachmentsInfo attchInfos)> GetAttchmentsInfoByBuildId(
            int buildId,
            string runNamePattern = "",
            bool toExclude = false)
        {
            var runIds = testResultDetailApiClient.GetRunIds(buildId);

            if (!string.IsNullOrEmpty(runNamePattern))
                runIds = SelectRunIdsByPattern(runIds, runNamePattern, toExclude);

            return GetAttchmentsInfoByRunIds(runIds);
        }

        private List<(int runId, AttachmentsInfo attchInfos)> GetAttchmentsInfoByRunIds(List<int> runIds)
            => runIds.Select(runId => (runId, testRunApiClient.GetAttachmentsInfo(runId))).ToList();

        private List<int> SelectRunIdsByPattern(
            List<int> initialRunIds,
            string runNamePattern,
            bool toExclude)
        {
            var runInfos = testRunApiClient.GetRunInfo(initialRunIds);
            var regex = new Regex(runNamePattern);
            var selectedRunInfos = toExclude
                ? runInfos.Where(runInfo => !regex.IsMatch(runInfo.name))
                : runInfos.Where(runInfo => regex.IsMatch(runInfo.name));
            var runIds = selectedRunInfos.Select(runInfo => runInfo.id).ToList();

            return runIds;
        }

        #endregion Trx

        #region JUnit

        public List<testsuites> GetJUnitAttachements(int buildId)
        {
            var allAtchsInfos = GetAttchmentsInfoByBuildId(buildId);
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

        private List<testsuites> GetJUnitAttachments(IEnumerable<(int runId, int attchId)> ids)
            => ids.Select(raId => testRunApiClient.GetJUnitReport(raId.runId, raId.attchId)).ToList();

        #endregion JUnit

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
    }
}
