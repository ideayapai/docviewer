using System;
using System.IO;
using Aspose.Slides;
using Aspose.Slides.Util;
using Common.Logging;
using Documents.Exceptions;
using Infrasturcture.Errors;

namespace Documents.Reader.Imp
{
    /// <summary>
    /// 读取PPT内容
    /// </summary>
    public class PPTReader: IReader
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public string Read(string filePath)
        {
            _logger.Debug("Read PPT File:" + filePath);

            try
            {
                Presentation presentation = new Presentation(filePath);
                return GetContent(presentation);
            }
            catch (Exception ex)
            {
                throw new ReadException(ErrorMessages.ReadExcelFailed, ex);
            }
           
        }

        public string Read(Stream inputStream)
        {
            try
            {
                Presentation presentation = new Presentation(inputStream);
                return GetContent(presentation);
            }
            catch (Exception ex)
            {
                throw new ReadException(ErrorMessages.ReadExcelFailed, ex);
            }
        }

        private static string GetContent(Presentation presentation)
        {
            string content = string.Empty;
            foreach (var slide in presentation.Slides)
            {
                ITextFrame[] textFramesSlideOne = SlideUtil.GetAllTextBoxes(slide);
                for (int i = 0; i < textFramesSlideOne.Length; i++)
                {
                    //Loop through paragraphs in current TextFrame
                    foreach (var para in textFramesSlideOne[i].Paragraphs)
                    {
                        //Loop through portions in the current Paragraph
                        foreach (var port in para.Portions)
                        {
                            //Display text in the current portion
                            content += port.Text;
                        }
                    }
                }
            }

            return content;
        }

        
    }
}