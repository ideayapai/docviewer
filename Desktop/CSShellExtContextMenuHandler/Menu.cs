using System;

namespace CSShellExtContextMenuHandler
{
    public enum MenuType
    {
        Normal,
        Separator,
    }

    public class Menu
    {
        public string Text { get; set; } //MenuText = "&上传文件";

        public IntPtr Bmp { get; set; }  //menuBmp = IntPtr.Zero;

        public string Verb { get; set; } //verb = "csdisplay";

        public string VerbCanonicalName { get; set; } //verbCanonicalName = "CSDisplayFileName";

        public string VerbHelpText { get; set; }//verbHelpText = "Display File Name (C#)";

        public uint CmdId { get; set; } //IDM_DISPLAY = 0;

        public uint Id { get; set; }

        public MenuType MenuType { get; set; }
    }
}
