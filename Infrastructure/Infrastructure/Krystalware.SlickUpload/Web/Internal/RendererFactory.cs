namespace Krystalware.SlickUpload.Web.Internal
{
    static class RendererFactory
    {
        public static IComponentRenderer GetRenderer(IRenderableComponent component)
        {
            // TODO: cache
            if (component is IFileSelector)
                return new FileSelectorRenderer();
            else if (component is IFileList)
                return new FileListRenderer();
            else if (component is IUploadProgressDisplay)
                return new UploadProgressDisplayRenderer();
            else if (component is IUploadConnector)
                return new UploadConnectorRenderer();
            else if (component is ISlickUpload)
                return new SlickUploadRenderer();
            else if (component is IMarkerComponent)
                return new MarkerComponentRenderer();
            else
                return null;
        }
    }
}
