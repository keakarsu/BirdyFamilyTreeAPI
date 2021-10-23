using BirdyModel.Birdy;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace BirdyModel.Database
{
    public class MesnetDbConnection
    {
        public DbConnection MyConnection { get; set; }
        public DbTransaction MyTransaction { get; set; }
        public List<ChangeLog> Changes { get; set; }
    }
}
