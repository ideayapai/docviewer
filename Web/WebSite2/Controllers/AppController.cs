using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite2.Controllers
{
    public class AppController : Controller
    {
        //
        // GET: /App/

        public ActionResult Index()
        {

           using (var ms = new MemoryStream())
            {
                string stringtest = "中国inghttp://www.baidu.com/mvc.test?&";
                QRCoderHelper.GetQRCode(stringtest, ms);
                Response.ContentType = "image/Png";
                Response.OutputStream.Write(ms.GetBuffer(), 0, (int)ms.Length);
                Response.End();
            }
            return View();
        }

    }


    public class QRCoderHelper {

        public static bool GetQRCode(string CodeText,System.IO.MemoryStream ms) 
        {

            ErrorCorrectionLevel Ecl = ErrorCorrectionLevel.M; //误差校正水平   
            string Content = strContent;//待编码内容  
            QuietZoneModules QuietZones = QuietZoneModules.Two;  //空白区域   
            int ModuleSize = 12;//大小  
            var encoder = new QrEncoder(Ecl);
            QrCode qr;
            if (encoder.TryEncode(Content, out qr))//对内容进行编码，并保存生成的矩阵  
            {
                var render = new GraphicsRenderer(new FixedModuleSize(ModuleSize, QuietZones));
                render.WriteToStream(qr.Matrix, ImageFormat.Png, ms);
            }
            else
            {
                return false;
            }
            return true;  
        }










        public static string strContent { get; set; }
    }
}
