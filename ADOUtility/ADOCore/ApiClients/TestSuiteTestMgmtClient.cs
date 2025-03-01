using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCore.ApiClients
{
    internal class TestSuiteTestMgmtClient
    {/*
        string organizationUrl = "https://dev.azure.com/{your-organization}";

        // Replace with your personal access token
        string personalAccessToken = "{your-personal-access-token}";

        // Replace with your project name
        string projectName = "{your-project-name}";

        // Replace with your test plan ID
        int testPlanId = { your - test - plan - id };

        // Replace with your query based suite ID
        int suiteId = { your - suite - id };

        // Create a connection to Azure DevOps
        VssConnection connection = new VssConnection(new Uri(organizationUrl), new VssBasicCredential(string.Empty, personalAccessToken));

        // Create a Test Management client
        TestManagementHttpClient testManagementClient = connection.GetClient<TestManagementHttpClient>();

            try
            {
                // Get the test suite
                TestSuite suite = testManagementClient.GetTestSuiteAsync(projectName, testPlanId, suiteId).Result;

                // Check if the suite is query based
                if (suite.SuiteType == "DynamicTestSuite")
                {
                    // Get the query
                    string query = suite.Query.Query;

        // Print the query
        Console.WriteLine("Query:");
                    Console.WriteLine(query);
                }
                else
                {
                    Console.WriteLine("The specified suite is not a query based suite.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }*/
    }
}
