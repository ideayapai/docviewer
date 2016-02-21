using System;
using System.IO;
using System.Runtime.InteropServices;
using Common.Logging;
using Infrasturcture.Errors;
using Microsoft.Office.Interop.Word;

namespace Infrasturcture.Converter
{
    /// <summary>
    /// 转换Word为PDF
    /// </summary>
    public class WordToPdfConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string sourcePath, string targetPath)
        {
            if (!File.Exists(sourcePath))
            {
                return ErrorMessages.FileNotExist;
            }

            ApplicationClass wordApplication = new ApplicationClass();
            Document wordDocument = null;
            object paramMissing = Type.Missing;
            Type wordType = wordApplication.GetType();

            try
            {
                object paramSourceDocPath = sourcePath;
                string paramExportFilePath = targetPath;

                //wordDocument = wordApplication.Documents.Open(
                //    ref paramSourceDocPath, ref paramMissing, ref paramMissing,
                //    ref paramMissing, ref paramMissing, ref paramMissing,
                //    ref paramMissing, ref paramMissing, ref paramMissing,
                //    ref paramMissing, ref paramMissing, ref paramMissing,
                //    ref paramMissing, ref paramMissing, ref paramMissing,
                //    ref paramMissing);

                
                Documents docs = wordApplication.Documents;
                Type docsType = docs.GetType();
                wordDocument = (Document)docsType.InvokeMember("Open", System.Reflection.BindingFlags.InvokeMethod, null, docs, new Object[] { sourcePath, true, true });
                
                if (wordDocument != null)
                {
                    WdExportFormat paramExportFormat = WdExportFormat.wdExportFormatPDF;
                    bool paramOpenAfterExport = false;
                    WdExportOptimizeFor paramExportOptimizeFor = WdExportOptimizeFor.wdExportOptimizeForPrint;
                    WdExportRange paramExportRange = WdExportRange.wdExportAllDocument;
                    int paramStartPage = 0;
                    int paramEndPage = 0;
                    WdExportItem paramExportItem = WdExportItem.wdExportDocumentContent;
                    bool paramIncludeDocProps = true;
                    bool paramKeepIRM = true;
                    WdExportCreateBookmarks paramCreateBookmarks = WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                    bool paramDocStructureTags = true;
                    bool paramBitmapMissingFonts = true;
                    bool paramUseISO19005_1 = false;

                    wordDocument.ExportAsFixedFormat(paramExportFilePath,
                                                    paramExportFormat, paramOpenAfterExport,
                                                    paramExportOptimizeFor, paramExportRange, paramStartPage,
                                                    paramEndPage, paramExportItem, paramIncludeDocProps,
                                                    paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
                                                    paramBitmapMissingFonts, paramUseISO19005_1,
                                                    ref paramMissing);
                }
                return ErrorMessages.ConvertSuccess;
            }
            catch (COMException ex)
            {
                _logger.Error(ex.StackTrace);
                _logger.Error(ex.Message);
                return ErrorMessages.OfficeToPdfUninstall;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return ErrorMessages.ConvertFailed;
            }
            finally
            {
                if (wordDocument != null)
                {
                    wordDocument.Close(ref paramMissing, ref paramMissing, ref paramMissing);
                }
                wordType.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, wordApplication, null);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
