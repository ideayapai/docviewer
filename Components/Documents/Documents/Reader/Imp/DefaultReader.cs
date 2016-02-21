using System.IO;
using Common.Logging;

namespace Documents.Reader.Imp
{
    public class DefaultReader: IReader
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public string Read(string sourcePath)
        {
            _logger.DebugFormat("文件{0} 不支持内容的提取", sourcePath);

            return string.Empty;
        }

        public string Read(Stream inputStream)
        {
            _logger.Debug("In DefaultReader");

            return string.Empty;
        }
    }
}
