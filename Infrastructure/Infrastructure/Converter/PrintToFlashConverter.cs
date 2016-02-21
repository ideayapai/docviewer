using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using Common.Logging;

namespace Infrasturcture.Converter
{
    /// <summary>
    /// 直接转换Office为Flash
    /// </summary>
    public class PrintToFlashConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public bool Convert(string sourcePath, string targetPath)
        {
            
            
            try
            {
                var process = new Process();
                //FlashPaper文件安装路径 可自行设置
                string flashPrintPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Tools\FlashPrinter.exe");
                //string flashPrintPath = "cmd.exe";
                process.StartInfo.FileName = flashPrintPath;
                process.StartInfo.Arguments = string.Format("{0} {1} -o {2}", flashPrintPath, sourcePath, targetPath);
                //process.StartInfo.UseShellExecute = false;
                //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //process.StartInfo.CreateNoWindow = true;
                //process.StartInfo.RedirectStandardOutput = true;
                //process.Start();


                //process.StartInfo.Arguments = string.Format("{0} {1} -o {2}", pdf2SwfexePath, sourcePath, targetPath);

                process.Start();
                process.WaitForExit();

                return File.Exists(targetPath);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.Message);
                _logger.Error(ex.StackTrace);

                //如果报错 将输出文件删除
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                return false;
            }
            //finally
            //{
            //    //如果进程还未结束，杀死进程树
            //    if (!process.HasExited)
            //    {
            //        KillProcess(process.Id);
            //    }
            //}
            
            
        }

        //杀死进程树
        private void KillProcess(int pid)
        {
            _logger.DebugFormat("Kill Process pid:{0}", pid);

            Process[] processes = Process.GetProcesses();
            foreach (Process t in processes)
            {
                if (GetParentProcess(t.Id) == pid)
                {
                    KillProcess(t.Id);
                }
            }

            try
            {
                Process myProc = Process.GetProcessById(pid);
                myProc.Kill();
            }
            catch (ArgumentException ex)
            {
                _logger.Error(ex.Message);
            }

        }

        //获取父进程
        private int GetParentProcess(int Id)
        {
            int parentPid = 0;
            using (var mo = new ManagementObject("win32_process.handle='" + Id.ToString(CultureInfo.InvariantCulture) + "'"))
            {
                try
                {
                    mo.Get();
                }
                catch (ManagementException ex)
                {
                    _logger.Error(ex.Message);
                    return -1;
                }
                parentPid = System.Convert.ToInt32(mo["ParentProcessId"], CultureInfo.InvariantCulture);
            }
            return parentPid;
        }
    }
}
