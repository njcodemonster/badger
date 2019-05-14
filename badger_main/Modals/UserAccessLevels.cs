using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace itemService_entity.Models
{
    [Table("user_access_levels")]
    public partial class UserAccessLevels
    {
        public int access_level_id { get; set; }
        public string access_level { get; set; }
        public string description { get; set; }
    }
}
