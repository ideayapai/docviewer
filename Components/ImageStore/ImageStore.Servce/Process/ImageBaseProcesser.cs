using ImageStore.Services.Context;

namespace ImageStore.Services.Process
{
    public abstract class ImageBaseProcesser
    {
        protected ImageBaseProcesser NextProcesser { get; set; }

        protected ImageBaseProcesser()
        {
            NextProcesser = new ImageEmptyProcesser();
        }

        protected ImageBaseProcesser(ImageBaseProcesser nextProcesser)
        {
            NextProcesser = nextProcesser;
        }

        public abstract int Process(UploadImageContext context);

        public ImageBaseProcesser Next(ImageBaseProcesser processer)
        {
            NextProcesser = processer;
            return NextProcesser;
        }
    }
}