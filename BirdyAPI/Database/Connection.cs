using BirdyModel.Birdy;
using BirdyModel.Database;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BirdyAPI.Database
{
    public class Connection
    {
        public static string ConnectionStringForLog = "Server=192.168.200.102;Port=5432;Database=system_log;User Id=sys_log;Password=2020Bikurti;";
        public static string ConnectionStringForMesnet = "Server=192.168.200.102;Port=5432;Database=mesnet;User Id=mesnet;Password='2020Bikurti';";
        public static string ConnectionStringForBirdy = "Server=192.168.200.102;Port=5432;Database=birdy;User Id=birdy_test;Password='2021!Win-vestaTE';";

        public static Func<DbConnection> ConnectionWinvestate;
        public static Func<DbConnection> ConnectionLog;
        public static Func<DbConnection> ConnectionMesnet;

        public static MesnetDbConnection StartTransaction()
        {
            var loDbObj = new MesnetDbConnection
            {
                MyConnection = ConnectionWinvestate()
            };

            loDbObj.MyConnection.Open();
            loDbObj.MyTransaction = loDbObj.MyConnection.BeginTransaction();

            return loDbObj;
        }

        public static void SaveChanges(MesnetDbConnection pConnection)
        {
            pConnection.MyTransaction.Commit();
            pConnection.MyConnection.Close();

            if (pConnection.Changes != null && pConnection.Changes.Any())
            {
                Task.Run(() => Crud<ChangeLog>.Insert(pConnection.Changes, out _));
            }
        }

        public static void DiscardChanges(MesnetDbConnection pConnection)
        {
            pConnection.MyTransaction.Rollback();
            pConnection.MyConnection.Close();
        }

        public static void PrepareDatabase()
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            ConnectionWinvestate = () => new NpgsqlConnection(ConnectionStringForBirdy);
            ConnectionLog = () => new NpgsqlConnection(ConnectionStringForLog);
            ConnectionMesnet = () => new NpgsqlConnection(ConnectionStringForMesnet);
        }

        public static bool OpenConnection(IDbConnection pDbConnection)
        {
            try
            {
                pDbConnection.Open();
                return true;
            }
            catch (Exception e)
            {
                try
                {
                    pDbConnection.Open();
                    return true;
                }
                catch (Exception)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }

        }
    }
}
