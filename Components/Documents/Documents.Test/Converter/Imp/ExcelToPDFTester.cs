using System;
using System.IO;
using Documents.Converter.Imp;
using Documents.Exceptions;
using Infrasturcture.Errors;
using NUnit.Framework;

namespace Documents.Test.Converter.Imp
{
    [TestFixture]
    public class ExcelToPdfTester
    {
        private readonly ExcelToPdfConverter _excelToPdf = new ExcelToPdfConverter();

        [Test]
        public void should_convert_from_excel_to_pdf()
        {
            string from = Environment.CurrentDirectory + @"\TestFiles\报价明细表.xlsx";
            string to = Environment.CurrentDirectory + @"\TestFiles\报价明细表excel.pdf";
            Assert.AreEqual(ErrorMessages.Success, _excelToPdf.Convert(from, to));
            Assert.IsTrue(File.Exists(to));
        }

        [Test]
        public void should_convert_from_97formatexcel_to_pdf()
        {
            string from = Environment.CurrentDirectory + @"\TestFiles\报价明细表97format.xls";
            string to = Environment.CurrentDirectory + @"\TestFiles\报价明细表97formatexcel.pdf";
         
            Assert.AreEqual(ErrorMessages.Success, _excelToPdf.Convert(from, to));
            Assert.IsTrue(File.Exists(to));
        }

        [Test]
        public void should_not_convert_from_excel_to_pdf_if_file_doesntexists()
        {
            string from = Environment.CurrentDirectory + @"\TestFiles\filennotexist.xlsx";
            string to = Environment.CurrentDirectory + @"\TestFiles\filennotexist.pdf";
         
            Assert.Catch(typeof (ConverterException), () => _excelToPdf.Convert(from,  to));

        }
    }
    
}
