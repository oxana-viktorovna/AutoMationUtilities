using Newtonsoft.Json.Linq;
using SwaggerParser.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SwaggerParser.Utilities
{
    internal class PostmanCollectionParser
    {
        public PostmanCollectionParser(string collectionName)
        {
            this.collectionName = collectionName;
        }

        private readonly string collectionName;

        internal List<ApiStat> ReadCollectionAsApiStat(string path)
        {
            var jObj = ReadCollection(path);
            return ConvertToApiStat(jObj);
        }

        private JObject ReadCollection(string path)
        {
            var filePath = Path.Combine(path, $"{collectionName}.postman_collection.json");
            var jsonfile = File.ReadAllText(filePath);

            return JObject.Parse(jsonfile);
        }

        private List<ApiStat> ConvertToApiStat(JObject collection)
        {
            var tokens = collection["item"].Select(i => i);
            var requests = GetRequest(tokens).ToList();
            var apiStats = requests.Where(r => r != null).Select(r => new ApiStat()
            {
                ApiName = collectionName,
                Tag = "",
                Method = r["method"].ToString(),
                Endpoint = UriParser.RemoveParams(r["url"]["raw"].ToString()).TrimEnd('/')
            });

            return apiStats.DistinctBy(a => new { a.Method, a.Endpoint }).ToList();
        }

        private IEnumerable<JToken>? GetRequest(IEnumerable<JToken>? tokens)
        {
            JToken request = null;
            foreach (var token in tokens)
            {
                request = token["request"];
                if (request == null)
                {
                    var items = token["item"].Select(i => i);
                    foreach (var item in GetRequest(items))
                    {
                        yield return item;
                    }
                }
                else
                {
                    yield return request;
                }
            }
        }
    }
}
