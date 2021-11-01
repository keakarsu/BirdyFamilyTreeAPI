using BirdyModel.Attributes;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BirdyModel.Database.Birdy
{
    public class Entity
    {
        [IgnoreLogging]
        [Key]
        public int id { get; set; }

        [LoggingPrimaryKey]
        public Guid row_guid { get; set; }

        [IgnoreLogging]
        public DateTime? row_create_date { get; set; }

        [IgnoreLogging]
        public DateTime? row_update_date { get; set; }

        [IgnoreLogging]
        public DateTime? row_delete_date { get; set; }

        [IgnoreLogging]
        public Guid? row_create_user { get; set; }

        [IgnoreLogging]
        public Guid? row_update_user { get; set; }

        [IgnoreLogging]
        public Guid? row_delete_user { get; set; }
        public bool? is_deleted { get; set; }
        public bool? is_active { get; set; }
    }
}
