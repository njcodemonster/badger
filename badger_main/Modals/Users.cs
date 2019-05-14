using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class Users
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Designation { get; set; }
        public int AccessLevelId { get; set; }
        public double? LastLogin { get; set; }
        public double? LastSession { get; set; }
        public int ActiveStatus { get; set; }
        public double CreatedAt { get; set; }
        public double? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
