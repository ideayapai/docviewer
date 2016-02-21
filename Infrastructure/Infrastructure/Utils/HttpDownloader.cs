using System;
using System.IO;
using System.Net;

namespace Infrasturcture.Utils
{
    /// <summary>
    /// 小文件下载器，不适合大文件下载.
    /// </summary>
    public class HttpDownloader
    {
        private const int BufferSize = 10240;

        public static void SaveAsFile(string uri, string fileName)
        {
            Uri downUri = new Uri(uri);
            
            //建立一个ＷＥＢ请求，返回HttpWebRequest对象           
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(downUri);
            
            //设置接收对象的范围为0-10000000字节。
            //hwr.AddRange(0, 10000000);

            //流对象使用完后自动关闭
            using (Stream stream = hwr.GetResponse().GetResponseStream())
            {
                //文件流，流信息读到文件流中，读完关闭
                using (FileStream fs = File.Create(fileName))
                {
                    //建立字节组，并设置它的大小是多少字节
                    byte[] bytes = new byte[BufferSize];
                    int n = 1;
                    while (n > 0)
                    {
                        //一次从流中读多少字节，并把值赋给Ｎ，当读完后，Ｎ为０,并退出循环
                        n = stream.Read(bytes, 0, BufferSize);
                        fs.Write(bytes, 0, n); //将指定字节的流信息写入文件流中
                    }
                }
            }

        }
    }
}
