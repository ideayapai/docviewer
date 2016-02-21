using System;
using System.Collections.Generic;
using System.Text;

namespace MQDemoProducer
{
    static class Debug
    {
        public static void Print(string format, params object[] args)
        {
            System.Diagnostics.Debug.Print(format, args);
        }

        public static void Print(string strText)
        {
            System.Diagnostics.Debug.Print("{0}", strText);
        }

        public static void PrintWithTime(string format, params object[] args)
        {
            format = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "]:" + format;
            System.Diagnostics.Debug.Print(format, args);
        }

        public static void PrintWithTime(string strText)
        {
            System.Diagnostics.Debug.Print("[{0}]:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), strText);
        }
    }
}
