using ADOCore;
using ADOCore.ApiClients;
using ADOCore.ApiClietns;
using ADOCore.Models;
using SharedCore.StringUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestRuns.Steps
{
    public class TestRunApiSteps
    {
        public TestRunApiSteps(AdoSettings adoSettings)
        {
            testRunApiClient = new TestRunApiClient(adoSettings);
            testResultDetailApiClient = new ResultDetailsByBuildApiClient(adoSettings);
            buildApiSteps = new BuildApiSteps(adoSettings);
        }

        private readonly TestRunApiClient testRunApiClient;
        private readonly ResultDetailsByBuildApiClient testResultDetailApiClient;
        private readonly BuildApiSteps buildApiSteps;

        #region Trx

        public List<TestRunUnitTestResult> GetTrxAttachments(int buildId, string? runNameFilter = null)
        {

            var buildName = buildApiSteps.GetBuildEnv(buildId);
            var runInfos = GetRunInfos(buildId);

            if (!string.IsNullOrEmpty(runNameFilter))
                runInfos = runInfos.Where(info => info.name.ToLower().Contains(runNameFilter.ToLower())).ToList();

            return GetTrxAttachments(runInfos);
        }

        public List<TestRunUnitTestResult> GetTrxAttachmentsByRunId(int runId)
        {
            var runInfo = GetRunInfo(runId);

            return GetTrxAttachments(new List<RunInfo> { runInfo });
        }

        public List<TestRunUnitTestResult> GetTrxAttachments(List<RunInfo> runInfos)
        {
            var runAttchmentsInfos = GetRunAttchmentsInfo(runInfos);
            var runAttchmentsIds = GetAttchsIdsByType(runAttchmentsInfos, ".trx");
            var testRunTrxAttachments = GetTestRunTrxAttachments(runAttchmentsIds);
            var testRunResults = testRunTrxAttachments
                .Where(trx => trx.testRunAttach != null && trx.testRunAttach.Results != null)
                .SelectMany(trx => {
                    var results = trx.testRunAttach.Results;
                    foreach (var result in results)
                    {
                        result.RunName = trx.runName;
                        var testNum_Str = result.testName.GetTestCaseNumber();
                        result.TestNumber = Int32.TryParse(testNum_Str, out int testnum) ? testnum : 0;
                    }

                    return results;
                });

            return testRunResults?.ToList();
        }

        private List<(string runName,TestRun testRunAttach)> GetTestRunTrxAttachments(IEnumerable<(RunInfo runInfo, int attchId)> ids)
            => ids.Select(rId => (rId.runInfo.name, testRunApiClient.GetTrxResults(rId.runInfo.id, rId.attchId))).ToList();

        private List<(RunInfo runInfo, AttachmentsInfo attchInfos)> GetRunAttchmentsInfo(List<RunInfo> runInfos)
            => runInfos.Select(runInfo => (runInfo, testRunApiClient.GetAttachmentsInfo(runInfo.id))).ToList();

        private RunInfo GetRunInfo(int runId)
            => testRunApiClient.GetRunInfo(runId);

        private List<RunInfo> GetRunInfos(int buildId)
        {
            var runIds = testResultDetailApiClient.GetRunIds(buildId);
            var runInfos = testRunApiClient.GetRunInfo(runIds);

            return runInfos;

        }

        #endregion Trx

        private static List<(RunInfo runInfo, int attchId)> GetAttchsIdsByType(IEnumerable<(RunInfo runInfo, AttachmentsInfo attchInfos)> runAttchInfos, string type)
        {
            var ids = new List<(RunInfo, int)>();
            foreach (var (runInfo, attchInfos) in runAttchInfos)
            {
                if (attchInfos == null)
                    continue;
                foreach (var attchInfo in attchInfos.value)
                {
                    if (attchInfo.fileName.Contains(type))
                        ids.Add((runInfo, attchInfo.id));
                }
            }

            return ids;
        }
    }
}
