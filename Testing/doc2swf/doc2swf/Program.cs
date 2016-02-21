using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

namespace doc2swf
{
    class Program
    {
        static void Main(string[] args)
        {

            string applicationPath = @"C:\Program Files (x86)\Print2Flash3\p2fServer.exe";

            string inFilePath = @"D:\practice\project\docviewer\doc2swf\testfile.doc";

            string outFilePath = @"D:\practice\project\docviewer\doc2swf\testfile.swf";

            ProcessStartInfo startInfo = new ProcessStartInfo(applicationPath);

            startInfo.Arguments = string.Concat(inFilePath, " ", outFilePath);

            Process myProcess = new Process();

            myProcess.StartInfo = startInfo;

            myProcess.Start();

            myProcess.Close();
        }
    }
}
