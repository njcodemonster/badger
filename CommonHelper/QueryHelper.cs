using System;
using System.Collections.Generic;

namespace CommonHelper
{
    public class QueryHelper
    {
        public  string MakeUpdateQuery(Dictionary<String,String> valuePairs, String table , String where)
        {
            String QueryToRetun = "update " + table + " set ";
            Boolean first = true;
            foreach (KeyValuePair<string, string> pair in valuePairs)
            {
                if (!first)
                {
                    QueryToRetun = QueryToRetun + ",";
                }
                first = false;
                QueryToRetun = QueryToRetun + pair.Key + "=\"" + pair.Value + "\"";
            }
            if(where != "")
            {
                QueryToRetun = QueryToRetun + " where " + where;
            }
            return QueryToRetun = QueryToRetun + ";";

        }
    }
}
