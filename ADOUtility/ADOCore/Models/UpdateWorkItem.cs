using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCore.Models
{
    public class RootUpdateWorkItem
    {
        public string Op { get; set; }
        public string Path { get; set; }
        public int Value { get; set; }
    }

    public class UpdateWorkItem
    {
        public string Op { get; set; }
        public string Path { get; set; }
        public string Value { get; set; }
    }
}
