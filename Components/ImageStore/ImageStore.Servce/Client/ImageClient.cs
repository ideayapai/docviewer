using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using Common.Logging;
using ImageStore.Services.Context;
using ImageStore.Services.Utils;

namespace ImageStore.Services.Client
{
    public class ImageClient
    {
        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

        public static UploadImageResult Upload(string fileName, NameValueCollection nvc)
        {
            using (FileStream imageStream = File.Open(fileName, FileMode.Open))
            {
                imageStream.Seek(0, SeekOrigin.Begin);
                var imageBytes = new byte[imageStream.Length];
                imageStream.Read(imageBytes, 0, (int)imageStream.Length);

                var formGenerator = new PostFormGenerator();
                // 所有表单数据
                var bytesList = new ArrayList();
                // 普通表单
                foreach (var key in nvc.AllKeys)
                {
                    bytesList.Add(formGenerator.CreateFieldData(key, nvc[key]));
                }

                // 上传表单
                bytesList.Add(formGenerator.CreateFieldData("ImageUpload", fileName, "multipart/form-data", imageBytes));
                // 合成所有表单并生成二进制数组
                byte[] allBytes = formGenerator.JoinBytes(bytesList);

                // 返回的内容
                byte[] responseBytes;

                //上传服务地址
                var serviceUrl = ConfigurationManager.AppSettings["UploadServiceUrl"];
                if (string.IsNullOrWhiteSpace(serviceUrl))
                {
                    Logger.Error("serviceUrl没有配置地址");

                    return new UploadImageResult
                    {
                        ReturnCode = -1
                    };
                }
                // 上传到指定Url
                bool success = formGenerator.UploadData(serviceUrl, allBytes, out responseBytes);

                if (!success)
                {
                    Logger.Error("Upload image failed! ");
                    var message = "Params: ";
                    foreach (var key in nvc.AllKeys)
                    {
                        message += string.Format("[{0}={1}]  ", key, nvc[key]);
                    }
                    Logger.Error("------" + message);

                    return new UploadImageResult
                    {
                        ReturnCode = -1
                    };
                }
                return JsonFormaterUtils.Deserialize<UploadImageResult>(Encoding.UTF8.GetString(responseBytes));
            }
        }
    }
}