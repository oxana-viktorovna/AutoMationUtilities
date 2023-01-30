using ADOCore;
using ADOCore.ApiClients;
using TestRuns.Utilities;

namespace TestRuns.Steps
{
    public class BuildApiSteps
    {
        public BuildApiSteps(AdoSettings adoSettings)
        {
            buildsApiClient = new BuildsApiClient(adoSettings);
        }

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

        public List<int> GetAllBuildsIds(List<int> buildIds, List<int> rerunsBuildIds)
        {
            var allRuns = new List<int>();
            allRuns.AddRange(buildIds);

            if (rerunsBuildIds is not null)
                allRuns.AddRange(rerunsBuildIds);

            return allRuns;
        }

        public string GetShortBuildName(int buildId)
        {
            var buildData = buildsApiClient.GetBuild(buildId);
            var buildNameHelper = new BuildNameHelper(buildData.buildNumber);
            var buildDate = buildData.startTime.ToShortDateString().Replace("/","-");

            return $"{buildNameHelper.Env}-{buildNameHelper.Browser}-{buildDate}";
        }

        public string GetBuildName(List<int> buildIds)
        {
            if (buildIds.Count == 1)
                return GetFullBuildName(buildIds[0]);
            else
                return GetShortBuildName(buildIds);
        }

        public string GetShortBuildName(List<int> buildIds)
        {
            var buildsData = buildIds.Select(buildId => buildsApiClient.GetBuild(buildId)).ToList();
            var buildNameHelpers = buildsData.Select(buildData => new BuildNameHelper(buildData.buildNumber)).ToList();
            var buildDate = buildsData[0].startTime.ToShortDateString().Replace("/", "-");
            var envs = string.Join("-", buildNameHelpers.Select(b => b.Env));

            return $"{envs}-{buildNameHelpers[0].Browser}-{buildDate}";
        }

        public string GetFullBuildName(int buildId)
        {
            var buildsData = buildsApiClient.GetBuild(buildId);

            return buildsData.buildNumber;
        }

        public string GetBuildEnv(int buildId)
        {
            var buildData = buildsApiClient.GetBuild(buildId);
            var buildNameHelper = new BuildNameHelper(buildData.buildNumber);

            return buildNameHelper.Env;
        }

        public string GetBuildLink(int buildId)
        {
            return $"https://dev.azure.com/tr-corp-legal-tracker/Tracker/_build/results?buildId={buildId}&view=ms.vss-test-web.build-test-results-tab";
        }
    }
}
