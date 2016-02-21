using System;
using System.Diagnostics;
using System.IO;
using Aspose.Pdf;
using Common.Logging;
using Documents.Exceptions;
using Infrasturcture.Errors;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// 转换PDF为HTML
    /// pdf2htmlEX --dest-dir <HTML生成文件夹路径> <PDF源文件路径> <HTML文件名>
    /// </summary>
    public class PdfToHtmlConverter : IConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        /// 用法: pdf2htmlEX [options] <input.pdf> [<output.html>]
        //-f,--first-page <int>         需要转换的起始页 (默认: 1)
        //-l,--last-page <int>          需要转换的最后一页 (默认: 2147483647)
        //--zoom <fp>                   缩放比例
        //--fit-width <fp>              适合宽度 <fp> 像素
        //--fit-height <fp>             适合高度 <fp> 像素
        //--use-cropbox <int>           使用剪切框 (default: 1)
        //--hdpi <fp>                   图像水平分辨率 (default: 144)
        //--vdpi <fp>                   图像垂直分辨率 (default: 144)
        //--embed <string>              指定哪些元素应该被嵌入到输出
        //--embed-css <int>             将CSS文件嵌入到输出中 (default: 1)
        //--embed-font <int>            将字体文件嵌入到输出中 (default: 1)
        //--embed-image <int>           将图片文件嵌入到输出中 (default: 1)
        //--embed-javascript <int>      将javascript文件嵌入到输出中 (default: 1)
        //--embed-outline <int>         将链接嵌入到输出中 (default: 1)
        //--split-pages <int>           将页面分割为单独的文件 (default: 0)
        //--dest-dir <string>           指定目标目录 (default: ".")
        //--css-filename <string>       生成的css文件的文件名 (default: "")
        //--page-filename <string>      分割的网页名称  (default:"")
        //--outline-filename <string>   生成的链接文件名称 (default:"")
        //--process-nontext <int>       渲染图行，文字除外 (default: 1)
        //--process-outline <int>       在html中显示链接 (default: 1)
        //--printing <int>              支持打印 (default: 1)
        //--fallback <int>              在备用模式下输出 (default: 0)
        //--embed-external-font <int>   嵌入局部匹配的外部字体 (default: 1)
        //--font-format <string>        嵌入的字体文件后缀 (ttf,otf,woff,svg) (default: "woff")
        //--decompose-ligature <int>    分解连字-> fi (default:0)
        //--auto-hint <int>             使用fontforge的autohint上的字体时不提示 (default: 0)
        //--external-hint-tool <string> 字体外部提示工具 (overrides --auto-hint) (default: "")
        //--stretch-narrow-glyph <int>  伸展狭窄的字形，而不是填充 (default: 0)
        //--squeeze-wide-glyph <int>    收缩较宽的字形，而不是截断 (default: 1)
        //--override-fstype <int>       clear the fstype bits in TTF/OTF fonts (default:0)
        //--process-type3 <int>         convert Type 3 fonts for web (experimental) (default: 0)
        //--heps <fp>                   合并文本的水平临界值，单位：像素(default: 1)
        //--veps <fp>                   vertical threshold for merging text, in pixels (default: 1)
        //--space-threshold <fp>        断字临界值 (临界值 * em) (default:0.125)
        //--font-size-multiplier <fp>   一个大于1的值增加渲染精度 (default: 4)
        //--space-as-offset <int>       把空格字符作为偏移量 (default: 0)
        //--tounicode <int>             如何处理ToUnicode的CMap (0=auto, 1=force,-1=ignore) (default: 0)
        //--optimize-text <int>         尽量减少用于文本的HTML元素的数目 (default: 0)
        //--bg-format <string>          指定背景图像格式 (default: "png")
        //-o,--owner-password <string>  所有者密码 (为了加密文件)
        //-u,--user-password <string>   用户密码 (为了加密文件)
        //--no-drm <int>                覆盖文档的 DRM 设置 (default: 0)
        //--clean-tmp <int>             转换后删除临时文件 (default: 1)
        //--data-dir <string>           指定的数据目录 (default: ".\share\pdf2htmlEX")
        //--debug <int>                 打印调试信息 (default: 0)
        //-v,--version                  打印版权和版本信息
        //-h,--help                     打印使用帮助信息

        //TODO
        //Copy right problem:http://sourceforge.net/p/pdftohtml/discussion/150221/thread/6c10ae7c/]
        //https://github.com/coolwanglu/pdf2htmlEX/wiki/Building
        readonly string pdf2HtmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                  @"Tools\Pdf\Pdf2htmlEX\pdf2htmlEX.exe");
        //public int Convert(string from, string to)
        //{
        //    _logger.DebugFormat("Pdf 转换为HTML:{0} 到 {1}", from, to);

        //    if (!File.Exists(pdf2HtmlPath))
        //    {
        //        throw new ConverterException(ErrorMessages.Pdf2SwfToolNotExist);
        //    }

        //    try
        //    {
        //        using (var process = new Process())
        //        {

        //            string workfolder = Path.GetDirectoryName(to);
        //            string fileName = Path.GetFileName(to);

        //            //--no-drm设置为1表示忽略原来文件的数字版权信息。
        //            string argements = string.Format("--no-drm 1 --zoom 2 --dest-dir {0} {1} {2}", workfolder, from, fileName);

        //            _logger.DebugFormat("调用 pdf2htmlEX 进行转换, pdf2HMTLPath:{0}, parameters:{1}", pdf2HtmlPath, argements);

        //            //调用新进程 进行转换
        //            var psi = new ProcessStartInfo(pdf2HtmlPath, argements);
        //            process.StartInfo = psi;
        //            process.StartInfo.UseShellExecute = false;
        //            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //            process.StartInfo.CreateNoWindow = true;
        //            process.Start();
        //            process.WaitForExit();
        //        }

        //        if (!File.Exists(to))
        //        {
        //            _logger.ErrorFormat("转换后的文件{0}后文件不存在", to);
        //            throw new ConverterException(ErrorMessages.ConvertedFileNotExist);
        //        }

        //        return ErrorMessages.Success;
        //    }

        //    catch (Exception ex)
        //    {
        //        throw new ConverterException(ErrorMessages.PdftoHtmlFailed, ex);
        //    }

        //}

        public int Convert(string from, string to)
        {
            _logger.DebugFormat("PDF转换为HTML, from:{0} to：{1}", from, to);

            try
            {
                var doc = new Document(from);
                doc.Save(to, SaveFormat.Html);
                return ErrorMessages.Success;
            }

            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.PdftoHtmlFailed, ex);
            }
        }

        public int Convert(Stream from, Stream to)
        {
            _logger.DebugFormat("PDF转换为HTML");

            try
            {
                var doc = new Document(from);
                doc.Save(to, SaveFormat.Html);
                return ErrorMessages.Success;
            }

            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.WordToHtmlFailed, ex);
            }
        }
    }
}
