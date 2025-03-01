using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;

namespace ADOCore.ApiClients
{
    public class BuildHttpClientService
    {
        public BuildHttpClientService(AdoSettings adoSettings)
        {
            projectName = adoSettings.Project;

            var connection = new VssConnection(new Uri(adoSettings.BaseOrgUrl), new VssBasicCredential(string.Empty, adoSettings.Password));
            client = connection.GetClient<BuildHttpClient>();
        }

        BuildHttpClient client;
        string projectName;

        /// <summary>
        /// Get latest completed build result.
        /// </summary>
        /// <param name="defenitionId">Build defenition Id.</param>
        /// <returns>Build result. Null if no build found.</returns>
        public BuildResult? GetBuildResult(int defenitionId)
        {
            var builds = client.GetBuildsAsync(
                    projectName,
                    definitions: new[] { defenitionId },
                    queryOrder: BuildQueryOrder.FinishTimeDescending,
                    top: 1
                ).Result;

            return builds.Any() ? client.GetBuildAsync(projectName, builds.First().Id).Result.Result : null;
        }
    }
}
