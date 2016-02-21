namespace WebSite2.Models
{
    public class BaseMenuViewModel
    {
        private MenuViewModel _menuViewModel;

        public MenuViewModel MenuViewModel
        {
            get { return _menuViewModel ?? (_menuViewModel = new MenuViewModel()); }
        }

        /// <summary>
        /// 设置选中的菜单
        /// </summary>
        private MenuType _activeMenuType;
        public MenuType ActiveMenuType
        {
            get { return _activeMenuType; }
            set
            {
                _activeMenuType = value;
                foreach (var menuViewItemModel in MenuViewModel.Menus)
                {
                    menuViewItemModel.IsActive = false;
                    if (menuViewItemModel.MenuType == _activeMenuType)
                    {
                        menuViewItemModel.IsActive = true;
                    }
                }
            }
        }
    }
}