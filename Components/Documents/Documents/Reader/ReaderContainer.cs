using System;
using System.Collections.Generic;
using Documents.Enums;

namespace Documents.Reader
{
    internal class ReaderContainer
    {
        private readonly Dictionary<DocumentType, Type> _dictionaries =
            new Dictionary<DocumentType, Type>();

        internal void Register(DocumentType filetype, Type reader)
        {
            _dictionaries.Add(filetype, reader);
        }

        internal IReader GetInstance(DocumentType documentType)
        {
            var readerType = _dictionaries[documentType];
            return Activator.CreateInstance(readerType) as IReader;
        }

    }
}
