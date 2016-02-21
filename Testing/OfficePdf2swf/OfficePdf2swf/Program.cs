using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

namespace OfficePdf2swf
{
    class Convert2Swf
    {
        public string inFilePath;
        public string outFilePath;
        public string applicationPath;

        public int ConvertFile()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(applicationPath);

            startInfo.Arguments = string.Concat(inFilePath, " ", outFilePath);

            Process myProcess = new Process();

            myProcess.StartInfo = startInfo;

            myProcess.Start();

            myProcess.Close();

            return 0;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string input = args[0];
            string output = args[1];
            string application = @"C:\Program Files (x86)\Print2Flash3\p2fServer.exe";

            Convert2Swf myConvert = new Convert2Swf();

            myConvert.inFilePath = input;
            myConvert.outFilePath = output;
            myConvert.applicationPath = application;

            myConvert.ConvertFile();
        }
    }
}
