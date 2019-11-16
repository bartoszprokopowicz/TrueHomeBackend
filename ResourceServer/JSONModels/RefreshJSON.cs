using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceServer.JSONModels
{
    public class RefreshJSON
    {
        public string Login { get; set; }
        public string RefreshToken { get; set; }
    }
}
