using ImageStore.Services.Context;

namespace ImageStore.UnitTest
{
    public static class UploadImageContextMaker
    {
        public static UploadImageContext MockImageContext(string fileName)
        {
            return new UploadImageContext(fileName);
        }

    }
}
