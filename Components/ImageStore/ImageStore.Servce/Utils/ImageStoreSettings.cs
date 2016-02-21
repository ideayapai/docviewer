using System.Configuration;

namespace ImageStore.Services.Utils
{
    public class ImageStoreSettings
    {
        public static string DatabaseConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ImageStore.DBConnection"].ConnectionString;
            }
        }

        public static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];

        }

        public static int MaxImageSize
        {
            get
            {
                return int.Parse(GetValue("ImageStore.MaxImageSize"));
            }
        }

        public static string SupportImageType
        {
            get
            {
                return GetValue("ImageStore.SupportImageType");
            }
        }

     
        public static string ImageDir
        {
            get
            {
                return GetValue("ImageStore.ImageDir");    
            }
        }

        public static string ImageUrl
        {
            get
            {
                return GetValue("ImageStore.ImageUrl");
            }
        }

        public static string CompressDir
        {
            get
            {
                return GetValue("ImageStore.CompressImageDir");
            }
        }

        public static string CompressUrl
        {
            get
            {
                return GetValue("ImageStore.CompressImageUrl");
            }
        }

        public static string ThumbDir
        {
            get
            {
                return GetValue("ImageStore.ThumbImageDir");
            }
        }

        public static string ThumbUrl
        {
            get
            {
                return GetValue("ImageStore.ThumbImageUrl");
            }
        }
    }
}
