using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCore.Models
{
    public class RootUpdateWorkItem
    {
        public string op { get; set; }
        public string path { get; set; }
        public int value { get; set; }
    }

    public class UpdateWorkItem
    {
        public string op { get; set; }
        public string path { get; set; }
        public object value { get; set; }
    }
}
