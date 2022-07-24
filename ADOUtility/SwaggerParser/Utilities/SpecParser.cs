using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SwaggerParser.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

namespace SwaggerParser.Utilities
{
    internal class SpecParser
    {
        internal dynamic ReadDynamicJson(string path, string apiName)
        {
            var filePath = Path.Combine(path, $"{apiName}.json");
            var jsonfile = File.ReadAllText(filePath);
            dynamic json = JsonConvert.DeserializeObject<ExpandoObject>(jsonfile, new ExpandoObjectConverter());

            return json;
        }

        internal List<ApiStat> SwaggerJsonToApiStat(string apiName, dynamic json)
        {
            var apistats = new List<ApiStat>();

            foreach (var path in json.paths)
            {
                foreach (var method in path.Value)
                {
                    var apistat = new ApiStat()
                    {
                        ApiName = apiName,
                        Tag = method.Value.tags[0],
                        Method = method.Key,
                        Endpoint = path.Key
                    };
                    apistats.Add(apistat);
                }
            }

            return apistats;
        }
    }
}
