using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSShellExtContextMenuHandler
{
    public static class LoggerManager
    {
        private static readonly Logger _logger = new Logger();

        public static Logger GetLogger()
        {
            return _logger;
        }

    }
}
