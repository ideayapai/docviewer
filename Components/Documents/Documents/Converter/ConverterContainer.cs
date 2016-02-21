using System;
using System.Collections.Generic;
using Documents.Enums;

namespace Documents.Converter
{
    internal class ConverterContainer
    {
        private readonly Dictionary<ConvertFileType, Type> _dictionaries = 
            new Dictionary<ConvertFileType, Type>();

        internal void Register(ConvertFileType filetype, Type converter)
        {
            _dictionaries.Add(filetype, converter);
        }

        internal IConverter GetInstance(ConvertFileType convertFileType)
        {
            var convererType = _dictionaries[convertFileType];
            return Activator.CreateInstance(convererType) as IConverter;
        }

    }
}
