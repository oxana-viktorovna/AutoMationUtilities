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
