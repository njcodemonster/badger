using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badgerApi.Interfaces
{
    public interface IEventRepo
    {
        Task<String> AddEventAsync<T>(T data, String tableName);
    }
    public class EventsRepo : IEventRepo
    {
        public async Task<string> AddEventAsync<T>(T data, string tableName)
        {
            return  "ssdsd";
        }
    }
}
