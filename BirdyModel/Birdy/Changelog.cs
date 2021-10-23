using BirdyModel.Attributes;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BirdyModel.Birdy
{
    [Table("birdy_change_log")]
    [IgnoreLogging]
    public class ChangeLog
    {
        [Key]
        public int id { get; set; }
        public Guid changed_user { get; set; }
        public string class_name { get; set; }
        public string property_name { get; set; }
        public string primary_key { get; set; }
        public string primary_key_field { get; set; }
        public string old_value { get; set; }
        public string new_value { get; set; }
        public string operation { get; set; }
        public DateTime date_changed { get; set; }
    }
}
