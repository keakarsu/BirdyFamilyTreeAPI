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
        public static List<BirdDto> GetAllBirdsByUserId(string pId)
        {
            var loQuery = Queries.GetAllBirdsByUserId;
            loQuery = loQuery.Replace("@P01", pId);

            using var connection = Connection.ConnectionBirdy();
            if (!Connection.OpenConnection(connection)) return null;

            var result = connection.Query<BirdDto>(loQuery).ToList();
            return !result.Any() ? new List<BirdDto>() : result;
        }

        public static BirdDto GetBirdById(string pId)
        {
            var loQuery = Queries.GetBirdByUserId;
            loQuery = loQuery.Replace("@P01", pId);

            using var connection = Connection.ConnectionBirdy();
            if (!Connection.OpenConnection(connection)) return null;

            var result = connection.Query<BirdDto>(loQuery).ToList();
            return !result.Any() ? null : result.First();
        }

    }
}
