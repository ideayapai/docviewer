namespace Krystalware.SlickUpload.Web.Internal
{
    interface ISlickUpload : IRenderableComponent
    {
        IFileSelector FileSelector { get; }
        IFileList FileList { get; }
        IUploadProgressDisplay UploadProgressDisplay { get; }
        IUploadConnector UploadConnector { get; }
    }
}
