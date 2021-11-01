using BirdyAPI.Controllers;
using BirdyAPI.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BirdyAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Common.InitVariablesForDebug();
            Common.InitVariablesForTest();
            Common.InitVariablesForProd();
            Connection.PrepareDatabase();

            BirdController loController = new BirdController();
            loController.GetFamilyTreeOfBird("377c4cbc-fc1f-4b8b-ab6e-06a0b796b486");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
