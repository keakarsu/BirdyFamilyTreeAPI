using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BirdyModel.Database.Birdy
{
    [Table("birdy_user")]
    public class User : Entity
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string phone { get; set; }
    }
}
