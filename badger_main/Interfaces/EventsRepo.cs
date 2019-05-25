using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badgerApi.Interfaces
{
    public interface IEventRepo
    {
        Task<String> AddEvent<T>(T data, String tableName);
    }
    public class EventsRepo : IEventRepo
    {
        public Task<string> AddEvent<T>(T data, string tableName)
        {
            return String;
        }
    }
}
