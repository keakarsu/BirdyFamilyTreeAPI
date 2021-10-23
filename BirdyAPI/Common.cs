using BirdyAPI.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BirdyAPI
{
    public static class Common
    {
        [Conditional("PROD")]
        public static void InitVariablesForProd()
        {
            Connection.ConnectionStringForMesnet = "Server=192.168.200.200;Port=5432;Database=mesnet;User Id=api_connector;Password=!Api_connector*!;";
            Connection.ConnectionStringForLog = "Server=192.168.200.200;Port=5432;Database=system_log;User Id=logger;Password=!loggerMbT*_;";
            Connection.ConnectionStringForBirdy = "Server=192.168.200.200;Port=5432;Database=winvestate;User Id=winvestate;Password=2021Winvestate*!-;";
        }

        [Conditional("TEST")]
        public static void InitVariablesForTest()
        {
            Connection.ConnectionStringForLog = "Server=192.168.200.102;Port=5432;Database=system_log;User Id=mesnet;Password=2019MsNt!!;";
            Connection.ConnectionStringForMesnet = "Server=192.168.200.102;Port=5432;Database=mesnet;User Id=mesnet;Password=2019MsNt!!;";
            Connection.ConnectionStringForBirdy = "Server=192.168.200.102;Port=5432;Database=birdy_test;User Id=mesnet;Password=2019MsNt!!;";
        }

        public static void InitVariablesForDebug()
        {
            Connection.ConnectionStringForLog = "Server=192.168.200.102;Port=5432;Database=system_log;User Id=mesnet;Password=2019MsNt!!;";
            Connection.ConnectionStringForMesnet = "Server=192.168.200.102;Port=5432;Database=mesnet;User Id=mesnet;Password=2019MsNt!!;";
            Connection.ConnectionStringForBirdy = "Server=192.168.200.102;Port=5432;Database=birdy_test;User Id=mesnet;Password=2019MsNt!!;";

        }
    }
}
