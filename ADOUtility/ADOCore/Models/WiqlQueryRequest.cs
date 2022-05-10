using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOCore.Models
{
    public class WiqlQueryRequest
    {
        public WiqlQueryRequest(string query)
        {
            Query = query;
        }
        public string Query { get; set; }
    }
}
