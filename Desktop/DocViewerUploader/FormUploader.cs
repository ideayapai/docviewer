using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using Common.Logging;

namespace DocViewerUploader
{
    /// <summary>
    /// 表单上传工具
    /// 代码示例 
    /// var nvc = new NameValueCollection {{"userId", Guid.NewGuid().ToString()},
    ///                                     {"userName", "admin"}};
    /// FormUploader fileUploader = new FormUploader("http://localhost:18889/api/Document/Add");
    /// fileUploader.Upload("pic.bmp", nvc);
    /// </summary>
    public class FormUploader
    {
        private readonly string _uploadServiceUrl;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uploadServiceUrl">连接的上传服务器API地址</param>
        public FormUploader(string uploadServiceUrl)
        {
            _uploadServiceUrl = uploadServiceUrl;
        }

        /// <summary>
        /// 本地文件和表单上传
        /// </summary>
        /// <param name="fileName">本地文件名称</param>
        /// <param name="nvc">表单参数</param>
        /// <returns></returns>
        public string Upload(string fileName, NameValueCollection nvc)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);

                return Upload(nvc, fileName, "application/octet-stream", buffer);
            }
        }

        /// <summary>
        /// 网络文件上传
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="imageName"></param>
        /// <param name="nvc"></param>
        /// <returns></returns>
        public string Upload(byte[] bytes, string imageName, NameValueCollection nvc)
        {
            return Upload(nvc, imageName, "application/octet-stream", bytes);
        }

        /// <summary>
        /// 网络文件和表单上传
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="nvc"></param>
        /// <returns></returns>
        public string Upload(HttpContext httpContext, NameValueCollection nvc)
        {
            var imageFile = httpContext.Request.Files[0];
            var inputStream = imageFile.InputStream;
            var fileName = imageFile.FileName;
            var contentType = imageFile.ContentType;

            inputStream.Seek(0, SeekOrigin.Begin);
            var imageBytes = new byte[inputStream.Length];
            inputStream.Read(imageBytes, 0, (int)inputStream.Length);

            return Upload(nvc, fileName, contentType, imageBytes);
        }

        private string Upload(NameValueCollection nvc, string fileName, string contentType, byte[] fileBytes)
        {
            var formGenerator = new PostFormGenerator();

            // 所有表单数据
            var bytesList = new ArrayList();

            // 普通表单
            foreach (var key in nvc.AllKeys)
            {
                bytesList.Add(formGenerator.CreateFieldData(key, nvc[key]));
            }

            // 上传表单
            bytesList.Add(formGenerator.CreateFieldData("ImageUpload", fileName, contentType, fileBytes));

            // 合成所有表单并生成二进制数组
            byte[] allBytes = formGenerator.JoinBytes(bytesList);

            // 返回的内容
            byte[] responseBytes;

            //上传服务地址


            // 上传到指定Url
            bool success = formGenerator.UploadData(_uploadServiceUrl, allBytes, out responseBytes);

            if (!success)
            {
                _logger.Error("Upload image failed! ");
                var message = "Params: ";
                foreach (var key in nvc.AllKeys)
                {
                    message += string.Format("[{0}={1}]  ", key, nvc[key]);
                }
                _logger.Error("------" + message);
            }
            return Encoding.UTF8.GetString(responseBytes);
        }
    }
}
