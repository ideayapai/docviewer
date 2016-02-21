using Documents.Converter;

namespace Services.Models
{
    /// <summary>
    /// 返回转换对象
    /// </summary>
    public class ConvertContract : BaseContract
    {
       
        public string SourceUrl { get; set; }

        public string ConvertUrl { get; set; }
    }
}