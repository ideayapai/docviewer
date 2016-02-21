using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Infrasturcture.DomainObjects
{
    public static class ConversionExtension
    {
        public static T ToEntity<T>(this object dto) where T:class 
        {
            return Convert<T>(dto) as T;
        }

        public static List<T> ToEntities<T>(this List<object> dtos) where T : class
        {
            return dtos.Select(dto => dto.ToEntity<T>()).ToList();
        }

        public static T ToObject<T>(this object dto) where T: class 
        {
            return Convert<T>(dto) as T;
        }

        public static List<T> ToObjects<T>(this List<object> dtos) where T : class
        {
            return dtos.Select(dto => dto.ToObject<T>()).ToList();
        }


        private static object Convert<T>(object dto) 
        {
            if (dto == null)
                return null;
            Type dtoEntity = dto.GetType();
            var piList = dtoEntity.GetProperties().Where(p => p.PropertyType.IsPublic == true).ToList();
            Assembly assembly = Assembly.GetAssembly(typeof (T));
            object resultObj = assembly.CreateInstance(String.Join(".", new string[] {typeof (T).Namespace, typeof (T).Name}));
            var piResultObj = resultObj.GetType().GetProperties().Where(p => p.PropertyType.IsPublic == true).ToList();
            foreach (PropertyInfo pi in piList)
            {
                var sourcePi = piResultObj.Find(p => p.Name == pi.Name);
                if (sourcePi != null)
                {
                    object value = pi.GetValue(dto, null);
                    if (sourcePi.CanWrite)
                    {
                        sourcePi.SetValue(resultObj, value, null);
                    }
                    
                }
            }
            return resultObj;
        }
    }
}
