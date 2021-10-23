using BirdyModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BirdyAPI.Database
{
    public static class GetData
    {
        public static int CheckApiKey(string pPassword)
        {
            var loQuery = Queries.CheckApiUser;
            loQuery = loQuery.Replace("@P01", pPassword);

            using var connection = Connection.ConnectionMesnet();

            var loUsers = connection.Query<ApiUser>(loQuery).ToList();
            var loUserId = loUsers.Count > 0 ? loUsers.First().id : 0;
            return !Connection.OpenConnection(connection) ? 0 : loUserId;
        }
    }
}
