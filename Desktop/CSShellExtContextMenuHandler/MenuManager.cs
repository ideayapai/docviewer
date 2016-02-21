
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CSShellExtContextMenuHandler
{
    public class MenuContext
    {
        public uint IdCmdFirst { get; set; }
    }

    public class MenuManager
    {
        private static readonly MenuManager _builder = new MenuManager();
        private List<Menu> _menus;
        public const int IDM_DISPLAY = 0; // 必须从0开始
        public const int IDM_DISPLAY2 = 1;

        public List<Menu> Menus
        {
            get { return _menus; }
        }
        /// <summary>
        /// 单例模式
        /// </summary>
        public static MenuManager Instance
        {
            get
            {
                return _builder;
            }
          
        }

        private MenuManager()
        {
     
        }


        public void Register(MenuContext context)
        {
            if (_menus == null || _menus.Count == 0)
            {
                _menus = new List<Menu>
                {
                    new Menu
                    {  
                        MenuType = MenuType.Separator,
                    },

                    new Menu
                    {

                        CmdId = context.IdCmdFirst + IDM_DISPLAY,
                        Id = IDM_DISPLAY,
                        Text = "上传2",
                        Verb = "upload",
                        VerbCanonicalName = "CSUpload",
                        VerbHelpText = "Upload File",
                        MenuType = MenuType.Normal,
                    },

                    new Menu
                    {
                        CmdId = context.IdCmdFirst + IDM_DISPLAY2,
                        Id = IDM_DISPLAY2,
                        Text = "系统配置...",
                        Verb = "config",
                        VerbCanonicalName = "CSConfig",
                        VerbHelpText = "Config File",
                        MenuType = MenuType.Normal,
                    },

                    new Menu
                    {
                      
                        MenuType = MenuType.Separator,
                    },
                };
              
            }
        }

        public uint GetHResultCode()
        {
            return IDM_DISPLAY2 - IDM_DISPLAY + 1;
        }

        public List<MENUITEMINFO> GetMenuItemInfo()
        {
            List<MENUITEMINFO> list = new List<MENUITEMINFO>();

            if (_menus == null || _menus.Count == 0)
            {
                return list;
            }

            foreach (var menu in _menus)
            {
                var menuInfo = new MENUITEMINFO();
                switch (menu.MenuType)
                {
                    case MenuType.Normal:
                        menuInfo.cbSize = (uint)Marshal.SizeOf(menuInfo);
                        menuInfo.fMask = MIIM.MIIM_BITMAP | MIIM.MIIM_SUBMENU | MIIM.MIIM_STRING | MIIM.MIIM_FTYPE |
                                            MIIM.MIIM_ID | MIIM.MIIM_STATE;
                        menuInfo.wID = menu.CmdId;
                        menuInfo.fType = MFT.MFT_STRING;
                        menuInfo.dwTypeData = menu.Text;
                        menuInfo.fState = MFS.MFS_ENABLED;
                        break;

                    case MenuType.Separator:
                        menuInfo.cbSize = (uint)Marshal.SizeOf(menuInfo);
                        menuInfo.fMask = MIIM.MIIM_TYPE;
                        menuInfo.fType = MFT.MFT_SEPARATOR;
                        break;
                }
               
                list.Add(menuInfo);
            }

            return list;
        }
    }
}
