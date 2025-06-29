using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharedCore.ResourcesUtilities
{
    public static class ResourcesUtilities
    {
        public static HashSet<int>? GetNewFeaturesIds(DateTime asof)
        {
            var allFeatureIds = GetAllNewFeaturesIds();
            DateTime closestDate = allFeatureIds.Keys.Where(date => date > asof).DefaultIfEmpty().Min();
            if (closestDate == default)
                closestDate = allFeatureIds.Keys.Max();

            return allFeatureIds[closestDate];
        }

        private static Dictionary<DateTime, HashSet<int>> GetAllNewFeaturesIds()
        {
            Dictionary<DateTime, HashSet<int>> data = new Dictionary<DateTime, HashSet<int>>();
            var embeddedResourcePath = @"SharedCore.Resources.NewFeaturesIds.csv";

            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(embeddedResourcePath))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(',');
                            if (parts.Length >= 2)
                            {
                                if (DateTime.TryParseExact(parts[0], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                                {
                                    HashSet<int> ids = new HashSet<int>();
                                    for (int i = 1; i < parts.Length; i++)
                                    {
                                        if (int.TryParse(parts[i], out int id))
                                        {
                                            ids.Add(id);
                                        }
                                    }
                                    data[date] = ids;
                                }
                            }
                        }
                    }
                }
            }

            return data;
        }
    }
}
