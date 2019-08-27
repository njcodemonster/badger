using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GenericModals
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
    public partial class LogiDetails
    {
        public String UserIdentity { get; set; }
        public String UserPass { get; set; }
    }
}
