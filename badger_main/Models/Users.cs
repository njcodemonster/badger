using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("users")]
    public partial class Users
    {
        [Key]
        public int user_id { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string full_name { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string designation { get; set; }
        public int access_level_id { get; set; }
        public double? last_login { get; set; }
        public double? last_session { get; set; }
        public int active_status { get; set; }
        public double created_at { get; set; }
        public double? updated_at { get; set; }
        public int created_by { get; set; }
        public int? updated_by { get; set; }
    }
}
