namespace DocViewerWinService.Handlers
{
    public interface IMessageHandler<T>
    {
        void Handle(T message);
    }
}
