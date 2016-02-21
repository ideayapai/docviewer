using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Documents;
using Documents.Enums;
using Services.Contracts;

namespace WebSite2.Controls
{
    public static class HtmlControlExtensions
    {
        public static MvcHtmlString Thumb(this HtmlHelper helper, DocumentObject document)
        {
            StringBuilder str = new StringBuilder();
            switch (document.DocumentType)
            {
                case DocumentType.BMP:
                    str.AppendFormat(
                    "<i class=\"file-small-icon file-small-icon-BMP\"><img src='{0}' alt='{1}' data-displaypath='{2}'> </i>",
                    document.ThumbUrl, document.FileName, document.DisplayPath);  
                    break;
                
                case DocumentType.JPG:
                    str.AppendFormat(
                    "<i class=\"file-small-icon file-small-icon-JPG\"><img src='{0}' alt='{1}' data-displaypath='{2}'> </i>",
                    document.ThumbUrl, document.FileName, document.DisplayPath);  
                    break;
                        
                case DocumentType.PNG:
                    str.AppendFormat(
                    "<i class=\"file-small-icon file-small-icon-PNG\"><img src='{0}' alt='{1}' data-displaypath='{2}'> </i>",
                    document.ThumbUrl, document.FileName, document.DisplayPath);  
                    break;

                case DocumentType.GIF:
                    str.AppendFormat(
                    "<i class=\"file-small-icon file-small-icon-GIF\"><img src='{0}' alt='{1}' data-displaypath='{2}'> </i>",
                    document.ThumbUrl, document.FileName, document.DisplayPath);
                    break; 

                case DocumentType.PSD:
                    str.Append("<i class=\"file-small-icon file-small-icon-PSD\"></i>");
                    break;

                case DocumentType.AI:
                    str.Append("<i class=\"file-small-icon file-small-icon-AI\"></i>");
                    break;

                case DocumentType.CAD:
                    str.AppendFormat(
                   "<i class=\"file-small-icon file-small-icon-CAD\"><img src='{0}' alt='{1}' data-displaypath='{2}'> </i>",
                   document.ThumbUrl, document.FileName, document.DisplayPath);
                    break;

                case DocumentType.TXT:
                    str.Append("<i class=\"file-small-icon file-small-icon-TXT\"></i>");
                    break;
                      
                case DocumentType.Word:
                    str.Append("<i class=\"file-small-icon file-small-icon-DOC\"></i>");
                    break;
                
                case DocumentType.Excel:
                    str.Append("<i class=\"file-small-icon file-small-icon-XLS\"></i>");
                    break;

                case DocumentType.PPT:
                    str.Append("<i class=\"file-small-icon file-small-icon-PPT\"></i>");
                    break;

                case DocumentType.ZIP:
                    str.Append("<i class=\"file-small-icon file-small-icon-ZIP\"></i>");
                    break;

                case DocumentType.RAR:
                    str.Append("<i class=\"file-small-icon file-small-icon-RAR\"></i>");
                    break;

                case DocumentType.JAR:
                    str.Append("<i class=\"file-small-icon file-small-icon-JAR\"></i>");
                    break;

                case DocumentType.DAMAGE:
                    str.Append("<i class=\"file-small-icon file-small-icon-DAMAGE\"></i>");
                    break;

                case DocumentType.ISO:
                    str.Append("<i class=\"file-small-icon file-small-icon-ISO\"></i>");
                    break;

                case DocumentType.OLD:
                    str.Append("<i class=\"file-small-icon file-small-icon-OLD\"></i>");
                    break;

                case DocumentType.Z7:
                    str.Append("<i class=\"file-small-icon file-small-icon-7Z\"></i>");
                    break;

                case DocumentType.BAK:
                    str.Append("<i class=\"file-small-icon file-small-icon-BAK\"></i>");
                    break;

                case DocumentType.TMP:
                    str.Append("<i class=\"file-small-icon file-small-icon-BAK\"></i>");
                    break;

                case DocumentType.HTML:
                    str.Append("<i class=\"file-small-icon file-small-icon-HTML\"></i>");
                    break;

                case DocumentType.HTM:
                    str.Append("<i class=\"file-small-icon file-small-icon-HTM\"></i>");
                    break;

                case DocumentType.XML:
                    str.Append("<i class=\"file-small-icon file-small-icon-XML\"></i>");
                    break;

                case DocumentType.ASP:
                    str.Append("<i class=\"file-small-icon file-small-icon-ASP\"></i>");
                    break;

                case DocumentType.PDF:
                    str.Append("<i class=\"file-small-icon file-small-icon-PDF\"></i>");
                    break;

                case DocumentType.CHM:
                    str.Append("<i class=\"file-small-icon file-small-icon-CHM\"></i>");
                    break;

                case DocumentType.Folder:
                    str.Append("<i class=\"file-small-icon file-small-icon-folders\"></i>");
                    break;

                case DocumentType.HLP:
                    str.Append("<i class=\"file-small-icon file-small-icon-HLP\"></i>");
                    break;

                case DocumentType.TTF:
                    str.Append("<i class=\"file-small-icon file-small-icon-TTF\"></i>");
                    break;

                case DocumentType.TTC:
                    str.Append("<i class=\"file-small-icon file-small-icon-TTC\"></i>");
                    break;
    
                case DocumentType.OTF:
                    str.Append("<i class=\"file-small-icon file-small-icon-OTF\"></i>");
                    break;

                case DocumentType.FON:
                    str.Append("<i class=\"file-small-icon file-small-icon-FON\"></i>");
                    break;

                case DocumentType.AVI:
                    str.Append("<i class=\"file-small-icon file-small-icon-AVI\"></i>");
                    break;

                case DocumentType.ASF:
                    str.Append("<i class=\"file-small-icon file-small-icon-ASF\"></i>");
                    break;
      
    
                case DocumentType.MP4:
                    str.Append("<i class=\"file-small-icon file-small-icon-MP4\"></i>");
                    break;
   
                case DocumentType.MP3:
                    str.Append("<i class=\"file-small-icon file-small-icon-MP3\"></i>");
                    break;

                case DocumentType.GP3:
                    str.Append("<i class=\"file-small-icon file-small-icon-GP3\"></i>");
                    break;

                case DocumentType.RMVB:
                    str.Append("<i class=\"file-small-icon file-small-icon-RMVB\"></i>");
                    break;

                case DocumentType.WMV:
                    str.Append("<i class=\"file-small-icon file-small-icon-WMV\"></i>");
                    break;
    
                case DocumentType.SWF:
                    str.Append("<i class=\"file-small-icon file-small-icon-SWF\"></i>");
                    break;

                case DocumentType.WMA:
                    str.Append("<i class=\"file-small-icon file-small-icon-WMA\"></i>");
                    break;

                case DocumentType.WAV:
                    str.Append("<i class=\"file-small-icon file-small-icon-WAV\"></i>");
                    break;
     
                case DocumentType.IPA:
                    str.Append("<i class=\"file-small-icon file-small-icon-IPA\"></i>");
                    break;
     
                case DocumentType.APK:
                    str.Append("<i class=\"file-small-icon file-small-icon-APK\"></i>");
                    break;
     
                case DocumentType.EXE:
                    str.Append("<i class=\"file-small-icon file-small-icon-EXE\"></i>");
                    break;   
                
                default:
                     str.Append("<i class=\"file-small-icon file-small-icon-FILE\"></i>");
        
                    break;
            }
          
            return new MvcHtmlString(str.ToString());
        }

        public static MvcHtmlString Icon(this HtmlHelper helper, DocumentObject document)
        {
            StringBuilder str = new StringBuilder();
            switch (document.DocumentType)
            {
                case DocumentType.BMP:
                    str.AppendFormat(
                    "<i class=\"file-icon file-icon-BMP\"><img src='{0}' alt='{1}' data-displaypath='{2}'> </i>",
                    document.ThumbUrl, document.FileName, document.DisplayPath);
                    break;

                case DocumentType.JPG:
                    str.AppendFormat(
                    "<i class=\"file-icon file-icon-JPG\"><img src='{0}' alt='{1}' data-displaypath='{2}'> </i>",
                    document.ThumbUrl, document.FileName, document.DisplayPath);
                    break;

                case DocumentType.PNG:
                    str.AppendFormat(
                    "<i class=\"file-icon file-icon-PNG\"><img src='{0}' alt='{1}' data-displaypath='{2}'> </i>",
                    document.ThumbUrl, document.FileName, document.DisplayPath);
                    break;

                case DocumentType.PSD:
                    str.Append("<i class=\"file-icon file-icon-PSD\"></i>");
                    break;

                case DocumentType.TXT:
                    str.Append("<i class=\"file-icon file-icon-TXT\"></i>");
                    break;

                case DocumentType.Word:
                    str.Append("<i class=\"file-icon file-icon-DOC\"></i>");
                    break;

                case DocumentType.Excel:
                    str.Append("<i class=\"file-icon file-icon-XLS\"></i>");
                    break;

                case DocumentType.PPT:
                    str.Append("<i class=\"file-icon file-icon-PPT\"></i>");
                    break;

                case DocumentType.PDF:
                    str.Append("<i class=\"file-icon file-icon-PDF\"></i>");
                    break;
                case DocumentType.Folder:
                    str.Append("<i class=\"file-icon file-icon-folders\"></i>");

                    break;
                case DocumentType.CAD:
                    str.AppendFormat(
                   "<i class=\"file-icon file-icon-CAD\"><img src='{0}' alt='{1}' data-displaypath='{2}'> </i>",
                   document.ThumbUrl, document.FileName, document.DisplayPath);
                    break;

                case DocumentType.ZIP:
                    str.Append("<i class=\"file-icon file-icon-ZIP\"></i>");
                    break;

                case DocumentType.RAR:
                    str.Append("<i class=\"file-icon file-icon-RAR\"></i>");
                    break;

                case DocumentType.JAR:
                    str.Append("<i class=\"file-icon file-icon-JAR\"></i>");
                    break;

                case DocumentType.DAMAGE:
                    str.Append("<i class=\"file-icon file-icon-DAMAGE\"></i>");
                    break;

                case DocumentType.ISO:
                    str.Append("<i class=\"file-icon file-icon-ISO\"></i>");
                    break;

                case DocumentType.OLD:
                    str.Append("<i class=\"file-icon file-icon-OLD\"></i>");
                    break;

                case DocumentType.Z7:
                    str.Append("<i class=\"file-icon file-icon-7Z\"></i>");
                    break;

                case DocumentType.BAK:
                    str.Append("<i class=\"file-icon file-icon-BAK\"></i>");
                    break;

                case DocumentType.TMP:
                    str.Append("<i class=\"file-icon file-icon-BAK\"></i>");
                    break;

                case DocumentType.HTML:
                    str.Append("<i class=\"file-icon file-icon-HTML\"></i>");
                    break;

                case DocumentType.HTM:
                    str.Append("<i class=\"file-icon file-icon-HTM\"></i>");
                    break;

                case DocumentType.XML:
                    str.Append("<i class=\"file-icon file-icon-XML\"></i>");
                    break;

                case DocumentType.ASP:
                    str.Append("<i class=\"file-icon file-icon-ASP\"></i>");
                    break;

                case DocumentType.CHM:
                    str.Append("<i class=\"file-icon file-icon-CHM\"></i>");
                    break;

                case DocumentType.HLP:
                    str.Append("<i class=\"file-icon file-icon-HLP\"></i>");
                    break;

                case DocumentType.TTF:
                    str.Append("<i class=\"file-icon file-icon-TTF\"></i>");
                    break;

                case DocumentType.TTC:
                    str.Append("<i class=\"file-icon file-icon-TTC\"></i>");
                    break;

                case DocumentType.OTF:
                    str.Append("<i class=\"file-icon file-icon-OTF\"></i>");
                    break;

                case DocumentType.FON:
                    str.Append("<i class=\"file-icon file-icon-FON\"></i>");
                    break;

                case DocumentType.AVI:
                    str.Append("<i class=\"file-icon file-icon-AVI\"></i>");
                    break;

                case DocumentType.ASF:
                    str.Append("<i class=\"file-icon file-icon-ASF\"></i>");
                    break;


                case DocumentType.MP4:
                    str.Append("<i class=\"file-icon file-icon-MP4\"></i>");
                    break;

                case DocumentType.MP3:
                    str.Append("<i class=\"file-icon file-icon-MP3\"></i>");
                    break;

                case DocumentType.GP3:
                    str.Append("<i class=\"file-icon file-icon-GP3\"></i>");
                    break;

                case DocumentType.RMVB:
                    str.Append("<i class=\"file-icon file-icon-RMVB\"></i>");
                    break;

                case DocumentType.WMV:
                    str.Append("<i class=\"file-icon file-icon-WMV\"></i>");
                    break;

                case DocumentType.SWF:
                    str.Append("<i class=\"file-icon file-icon-SWF\"></i>");
                    break;

                case DocumentType.WMA:
                    str.Append("<i class=\"file-icon file-icon-WMA\"></i>");
                    break;

                case DocumentType.WAV:
                    str.Append("<i class=\"file-icon file-icon-WAV\"></i>");
                    break;

                case DocumentType.IPA:
                    str.Append("<i class=\"file-icon file-icon-IPA\"></i>");
                    break;

                case DocumentType.APK:
                    str.Append("<i class=\"file-icon file-icon-APK\"></i>");
                    break;

                case DocumentType.EXE:
                    str.Append("<i class=\"file-icon file-icon-EXE\"></i>");
                    break;   
                default:
                    str.Append("<i class=\"file-icon file-icon-FILE\"></i>");
                    break;
            }

            return new MvcHtmlString(str.ToString());
        }
    }
}