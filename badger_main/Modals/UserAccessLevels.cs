using System;
using System.Collections.Generic;

namespace itemService_entity.Models
{
    public partial class UserAccessLevels
    {
        public int AccessLevelId { get; set; }
        public string AccessLevel { get; set; }
        public string Description { get; set; }
    }
}
