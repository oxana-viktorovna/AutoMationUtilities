using System;
using System.Collections.Generic;

namespace ADOCore.Models.WiqlQuery
{
    public class WiqlFlatQuery : ICloneable
    {
        public WiqlFlatQuery()
        {
            MainTable = "workitems";
        }

        public List<string> SelectAttributes { get; set; }

        public List<string> Conditions { get; set; }

        public string MainTable { get; set; }

        public string MergedQuery => $"SELECT {string.Join(",", SelectAttributes)} FROM {MainTable} WHERE {string.Join(" ", Conditions)}";

        public object Clone()
        {
            var clonned = new WiqlFlatQuery()
            {
                SelectAttributes = new List<string>(SelectAttributes),
                Conditions = new List<string>(Conditions)
            };

            return clonned;
        }
    }
}
