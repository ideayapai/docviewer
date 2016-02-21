using Documents.Enums;

namespace Documents.Converter
{
    internal static class ConverterContainerExtension
    {
        public static void Register<U>(this ConverterContainer container, ConvertFileType convertFileType)where U: IConverter
        {
            container.Register(convertFileType, typeof(U));
        }

        public static T Resolve<T>(this ConverterContainer container, ConvertFileType convertFileType) where T : IConverter
        {
            return (T)container.GetInstance(convertFileType);
        }

    }
}
