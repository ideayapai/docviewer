using System;
using System.Linq;

namespace SWFConverter
{
    class Program
    {
        static readonly SwfConverter _converter = new SwfConverter();

        static void Main(string[] args)
        {
            if (args.Count() < 2)
            {
                Console.WriteLine("转换参数不对");
            }
            else
            {
                _converter.Convert(args[0], args[1]);    
            }
            
        }
    }
}
