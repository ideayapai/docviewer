namespace DocViewerWinService.Jobs
{
    public class JobStatus
    {
        public static Status TransformStatus;
    }

    public enum Status
    {
        Off,    //自动转换进行中
        On      //空闲
    }
}
