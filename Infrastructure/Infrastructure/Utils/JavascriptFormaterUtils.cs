/*==============================================================================
* Copyright (c) cndotnet.org Corporation.  All rights reserved.
*===============================================================================
* This code and information is provided "as is" without warranty of any kind,
* either expressed or implied, including but not limited to the implied warranties
* of merchantability and fitness for a particular purpose.
*===============================================================================
* Licensed under the GNU General Public License (GPL) v2
* http://www.cndotnet.org/ezsocio
*==============================================================================*/

using System;
using System.Web.Script.Serialization;

namespace Infrasturcture.Utils
{
    /// <summary>
    /// Json formater helper class
    /// </summary>
    public static class JavascriptFormaterUtils
    {
       
        /// <summary>
        /// Serialize a object to json data
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object instance</param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            var jsonFormater = new JavaScriptSerializer();
            return jsonFormater.Serialize(obj);
               
        }

        /// <summary>
        /// Deserialize json data to a object using default encoding
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="jsonDatas">Json data</param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsonDatas)
        {
            try
            {
                var jsonFormater = new JavaScriptSerializer();

                return (T)jsonFormater.Deserialize(jsonDatas, typeof(T));
            }
            catch(Exception  e)
            {
                return default(T);
            }
        }

    }
}