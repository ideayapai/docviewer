using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSShellExtContextMenuHandler
{
    public class Logger
    {
        private string _path;

        public Logger()
        {
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"\log\");
            //判断Log目录是否存在，不存在则创建
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string date = DateTime.Now.ToString("yyyyMMdd");

            _path = Path.Combine(directory, date + ".log");
        }

        public void Info(string msg)
        {
            //使用StreamWriter写日志，包含时间，错误路径，错误信息
            using (StreamWriter sw = File.AppendText(_path))
            {
                sw.WriteLine("-----------------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-----------------");
                sw.WriteLine(msg);
                sw.WriteLine("\r\n");
            }
        }
    }
}
