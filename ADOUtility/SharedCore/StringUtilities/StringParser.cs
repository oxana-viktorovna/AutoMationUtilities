using System;
using System.Text.RegularExpressions;

namespace SharedCore.StringUtilities
{
    public static class StringParser
    {
        public static string GetTestCaseNumber(this string testMethod)
        {
            var regex = new Regex(@"T\d+_");
            var number = regex.Match(testMethod).Groups[0].Value.Replace("T", "").Replace("_", "");

            return number;
        }

        public static int GetTcNumber(this string testMethod)
        {
            var regex = new Regex(@"T\d+_");
            var number = regex.Match(testMethod).Groups[0].Value.Replace("T", "").Replace("_", "");

            return Convert.ToInt32(number);
        }

        public static string GetTestMethodName(this string testMethod)
            => Regex.Replace(testMethod,@"([TP]\d*_)+","");
    }
}
