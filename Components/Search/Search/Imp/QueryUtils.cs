using System;
using System.Collections.Generic;

namespace Search.Imp
{
    public class QueryUtils
    {
       
        /// <summary>
        /// 获取查询字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string[] GetQueryStrings(Type type)
        {
            var queryStrings = new List<string>();

            foreach (var property in type.GetProperties())
            {
                foreach (var customAttribute in property.GetCustomAttributes(true))
                {
                    var attribute = customAttribute as SearchIndexAttribute;
                    if (attribute != null)
                    {
                        queryStrings.Add(attribute.Name ?? property.Name);
                        break;
                    }
                }
            }
            return queryStrings.ToArray();
        }

        
       
    }
}
