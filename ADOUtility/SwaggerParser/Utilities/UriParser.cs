namespace SwaggerParser.Utilities
{
    internal class UriParser
    {
        internal static string RemoveParams(string uri)
        {
            var n = uri.IndexOf('?');

            return n == -1
                ? uri
                : uri.Substring(0, n);
        }
    }
}
