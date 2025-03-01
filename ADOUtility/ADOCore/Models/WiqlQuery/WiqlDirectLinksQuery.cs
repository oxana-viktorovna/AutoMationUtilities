using System;
using System.Collections.Generic;
using System.Linq;

namespace ADOCore.Models.WiqlQuery
{
    public class WiqlDirectLinksQuery : ICloneable
    {
        public WiqlDirectLinksQuery()
        {
            this.mainTable = "workitemLinks";
        }

        public List<string> SelectAttributes { get; set; }
        public List<string> SourceConditions { get; set; }
        public List<string> TargetConditions { get; set; }
        public string Mode { get; set; }

        private string mainTable;

        public string MergedQuery => $@"SELECT {string.Join(",", SelectAttributes)} 
        FROM {mainTable}
        WHERE
            ({string.Join(' ', SourceConditions)})
            AND ({string.Join(' ', TargetConditions)})
        order by {SelectAttributes.First()}
        MODE ({Mode})";

        public object Clone()
        {
            var clonned = new WiqlDirectLinksQuery()
            {
                SelectAttributes = new List<string>(SelectAttributes),
                SourceConditions = new List<string>(SourceConditions),
                TargetConditions = new List<string>(TargetConditions),
                Mode = Mode
            };

            return clonned;
        }
    }
}
