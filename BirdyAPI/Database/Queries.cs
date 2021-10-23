using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BirdyAPI.Database
{
    public class Queries
    {
        public static string CheckApiUser = "Select * from apiuser where api_key='@P01' and application='BirdyAPI'";

    }
}
