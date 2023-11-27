using ADOCore;
using ADOCore.ApiClients;
using TestCases.Utilities;

namespace TestCases.Steps
{
    public class BuildApiSteps
    {
        public BuildApiSteps(AdoSettings adoSettings)
        {
            buildsApiClient = new BuildsApiClient(adoSettings);
        }

        private readonly BuildsApiClient buildsApiClient;
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
    }
}
