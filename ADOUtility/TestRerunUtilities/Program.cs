// See https://aka.ms/new-console-template for more information
using ADOCore;
using ADOCore.Models;
using SharedCore.Settings;
using System.Text;
using TestRuns.Models;
using TestRuns.Steps;

Console.WriteLine("Welcome");
Console.WriteLine("Please provide ADO build NUMBER (from URL) to get failed tests from:");
var buildNumber = Console.ReadLine();
while (IsBuildNumInValid(buildNumber))
{
    Console.WriteLine("Please provide valid ADO build NUMBER (integer)");
    buildNumber = Console.ReadLine();
}
var rerunStr = GetFailedUiTests(Convert.ToInt32(buildNumber));

Console.WriteLine("Here is your test filter for re-run:");
Console.WriteLine(rerunStr);

Console.ReadKey();

bool IsBuildNumInValid(string buildNumber)
{
    return string.IsNullOrEmpty(buildNumber) || !int.TryParse(buildNumber, out _);
}

string GetFailedUiTests(int buildNumber)
{
    var adoSettingsReader = new SettingsReader("ADOconfig.json");
    var adoSettings = new AdoSettings(adoSettingsReader);
    var apiSteps = new TestRunApiSteps(adoSettings);

    var testResults = apiSteps.GetTrxAttachements(buildNumber);
    var uiFailedTests = testResults.GetAllRunResults();

    var rerunStr = new StringBuilder();
    rerunStr.Append("&(");
    foreach (var uiFailedTest in uiFailedTests)
    {
        rerunStr.Append($"Name~{uiFailedTest.testName}|");
    }
    rerunStr.Remove(rerunStr.Length - 1, 1); // remove last |
    rerunStr.Append(")");

    return rerunStr.ToString();
}
