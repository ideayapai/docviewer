using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace Krystalware.SlickUpload
{
	/// <summary>
	/// A simple file-based log class.
	/// </summary>
	internal sealed class SimpleLogger
	{
        static object _lock = new object();

		private SimpleLogger()
		{}

		public static void Log(string message)
		{
#if DEBUG
			if (LogLocation != null && LogLocation.Length > 0)
			{
                lock (_lock)
                {
                    Directory.CreateDirectory(LogLocation);

                    string fileName = Path.Combine(LogLocation, "su-" + System.Diagnostics.Process.GetCurrentProcess().Id + "-" + DateTime.Now.ToString("yyyyMMMdd_HH") + ".log");

				    StreamWriter writer = null;

				    try
				    {
					    writer = File.AppendText(fileName);

					    writer.WriteLine(message + " @ " + DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"));
				    }
				    finally
				    {
					    if (writer != null)
						    writer.Close();
				    }
                }
			}
#endif
		}

        public static void Log(string message, string uploadId)
        {
#if DEBUG
            Log(uploadId + ":" + message);
#endif
        }

        public static void Log(HttpWorkerRequest worker, string uploadId)
        {
#if DEBUG
            Log(" - Content-Length:" + worker.GetKnownRequestHeader(System.Web.HttpWorkerRequest.HeaderContentLength), uploadId);
#endif
        }

        public static void Log(UploadedFile file, string uploadId)
        {
#if DEBUG
            Log(" - Client FileName:" + file.ClientName);

            //if (file.Location is FileUploadLocation)
            //    Log(" - Server FileName:" + ((FileUploadLocation)file.Location).FileName);
#endif
        }

        static string LogLocation
		{
			get
			{
                string path;

                path = ConfigurationManager.AppSettings["logLocation"];

                if (!string.IsNullOrEmpty(path) && !Path.IsPathRooted(path))
                    path = HostingEnvironment.MapPath(path);

                return path;
			}
		}
	}
}
