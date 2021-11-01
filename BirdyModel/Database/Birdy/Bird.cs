using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BirdyModel.Database.Birdy
{
    [Table("birdy_bird")]
    public class Bird : Entity
    {
        public string identity_no { get; set; }
        public string poultry_identity { get; set; }
        public string identity_color { get; set; }
        public DateTime? poultry_enter_date { get; set; }
        public DateTime? birth_date { get; set; }
        public string gender { get; set; }
        public string physical_attribute { get; set; }
        public string physical_property { get; set; }
        public string flying_attribute { get; set; }
        public string old_owner_name { get; set; }
        public string story { get; set; }
        public Guid? mother_uuid { get; set; }
        public Guid? father_uuid { get; set; }
        public Guid? user_uuid { get; set; }
        public string nickname { get; set; }
    }
}
