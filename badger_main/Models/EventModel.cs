using CommonHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badgerApi.Models
{
    public class EventModel
    {
        public EventModel(string tableName)
        {
            Table = tableName;
            CreatedAt = CommonHelper.CommonHelper.GetTimeStamp();
        }
        public int EventId { get; set; }
        public int EntityId { get; set; }
        public EventTypeModel EventType { get; set; }
        public int RefrenceId { get; set; }
        public string EventNotes { get; set; }
        public int UserId { get; set; }
        public double CreatedAt { get; set; }
        public string Table { get; set; }
        
    }
}
