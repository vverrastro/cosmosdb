using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmosdb.Web.Configuration
{
    public class DatabaseOptions
    {
        public string Endpoint { get; set; }
        public string AuthKey { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
    }
}
