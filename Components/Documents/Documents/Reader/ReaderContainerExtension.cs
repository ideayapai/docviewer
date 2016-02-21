using Documents.Converter;
using Documents.Enums;

namespace Documents.Reader
{
    internal static class ReaderExtension
    {
        public static void Register<U>(this ReaderContainer container, DocumentType documentType)where U: IReader
        {
            container.Register(documentType, typeof(U));
        }

        public static T Resolve<T>(this ReaderContainer container, DocumentType documentType) where T : IReader
        {
            return (T)container.GetInstance(documentType);
        }

    }
}
