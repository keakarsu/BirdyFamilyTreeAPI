using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BirdyAPI.Database
{
    public class Queries
    {
        public static string CheckApiUser = "Select * from apiuser where api_key='@P01' and application='BirdyAPI'";
        public static string GetAllBirdsByUserId = "select * from vw_birds where coalesce(is_deleted,false) =false and user_uuid::text ='@P01'";
        public static string GetBirdByUserId = "select * from vw_birds where coalesce(is_deleted,false) =false and row_guid::text ='@P01'";

    }
}
