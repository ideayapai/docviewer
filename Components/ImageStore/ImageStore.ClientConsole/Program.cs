using System;
using System.Collections.Specialized;
using ImageStore.Services.Client;
using ImageStore.Services.Repository;

namespace ImageStore.ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("上传图片");
            var nvc = new NameValueCollection {{"deviceId", "2"}};
            var result = ImageClient.Upload(@"C:\Users\Administrator\Desktop\公共数据库系统建设\新建文件夹\slide_02.png", new NameValueCollection());
            Console.WriteLine(result);

            Console.WriteLine("写入数据库");
            var entity = new Images
                             {
                                 ImageUrl = result.ImageUrl,
                             };
            var providre = new ImageDataRepository();
            providre.Add(entity);

            Console.ReadLine();
        }
    }
}
