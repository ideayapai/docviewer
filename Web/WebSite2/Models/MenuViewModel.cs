using System.Collections.Generic;
using System.Security.Policy;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;

namespace WebSite2.Models
{
    public enum MenuType
    {
        All = 0,
        Recent,
        Office,
        Cad,
        Image,
        Trash,
        Setting,
        AccountSetting,
        UserRoleSetting,
        AreaSetting,
        DepartmentSetting,
        RoleSetting,
        AuthSetting,
    }

    public class MenuViewItemModel
    {
        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType MenuType { get; set; }
        
        /// <summary>
        /// 菜单名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }
        
        /// <summary>
        /// 菜单链接
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 桥梁子菜单
        /// </summary>
        private List<MenuViewItemModel> _childMenus = new List<MenuViewItemModel>();

        public List<MenuViewItemModel> ChildMenus
        {
            get { return _childMenus ?? (_childMenus = new List<MenuViewItemModel>()); }
            set { _childMenus = value; }
        }
    }

    public class MenuViewModel
    {
        private List<MenuViewItemModel> _menus = new List<MenuViewItemModel>();

        /// <summary>
        /// 设置选中的菜单
        /// </summary>
        private MenuType _activeMenuType;
        public MenuType ActiveMenuType
        {
            get { return _activeMenuType; }
            set { 
                _activeMenuType = value;
                foreach (var menuViewItemModel in Menus)
                {
                    menuViewItemModel.IsActive = false;
                    if (menuViewItemModel.MenuType == _activeMenuType)
                    {
                        menuViewItemModel.IsActive = true;
                    }
                }
            }
        }

        /// <summary>
        /// 所有菜单
        /// </summary>
        public List<MenuViewItemModel> Menus
        {
            get
            {
                if (_menus == null || _menus.Count == 0)
                {
                    _menus = new List<MenuViewItemModel>
                                 {
                                  
                                      new MenuViewItemModel
                                         {
                                             Icon = "icon-menu icon-menu-2 micon",
                                             IsActive = false,
                                             MenuType = MenuType.All,
                                             Name = "所有空间",
                                             Url = "/",
                                         },
                                    new MenuViewItemModel
                                         {
                                             Icon = "icon-menu icon-menu-3 micon",
                                             IsActive = false,
                                             MenuType = MenuType.Recent,
                                             Name = "最近",
                                             Url = "/Document/Recent",
                                         },
                                   
                                     new MenuViewItemModel
                                         {
                                             Icon = "icon-menu icon-menu-4 micon",
                                             IsActive = false,
                                             MenuType = MenuType.Cad,
                                             Name = "工程图",
                                             Url = "/Document/CAD",
                                         },
                                      new MenuViewItemModel
                                         {
                                             Icon = "icon-menu icon-menu-5 micon",
                                             IsActive = false,
                                             MenuType = MenuType.Office,
                                             Name = "文档",
                                             Url = "/Document/Office",
                                         },
                                     new MenuViewItemModel
                                         {
                                             Icon = "icon-menu icon-menu-6 micon",
                                             IsActive = false,
                                             MenuType = MenuType.Image,
                                             Name = "图片",
                                             Url = "/Document/Image",
                                         },
                                     new MenuViewItemModel
                                         {
                                             Icon = "icon-menu icon-menu-7 micon",
                                             IsActive = false,
                                             MenuType = MenuType.Trash,
                                             Name = "回收站",
                                             Url = "/Document/Trash",
                                         },

                                     new MenuViewItemModel
                                         {
                                             Icon = "icon-menu icon-menu-8 micon",
                                             IsActive = false,
                                             MenuType = MenuType.AccountSetting,
                                             Name = "系统配置",
                                             Url = "/Setting/Index",
                                         }
                                     //new MenuViewItemModel
                                     //    {
                                     //        Icon = "icon icon-setting micon",
                                     //        IsActive = false,
                                     //        MenuType = MenuType.AccountSetting,
                                     //        Name = "设置",
                                     //        Url = "/Setting/Index",
                                            
                                     //    },
                                     //  new MenuViewItemModel
                                     //    {
                                     //        Icon = "icon icon-setting micon",
                                     //        IsActive = false,
                                     //        MenuType = MenuType.AccountSetting,
                                     //        Name = "用户&权限",
                                     //        ChildMenus = new List<MenuViewItemModel>
                                     //        {
                                     //            new MenuViewItemModel
                                     //            {
                                     //                Icon = "icon icon-setting micon",
                                     //                IsActive = false,
                                     //                MenuType = MenuType.AreaSetting,
                                     //                Name = "区域管理",
                                     //                Url = "/Area/Index",
                                     //            },

                                     //            new MenuViewItemModel
                                     //            {
                                     //                Icon = "icon icon-setting micon",
                                     //                IsActive = false,
                                     //                MenuType = MenuType.DepartmentSetting,
                                     //                Name = "部门管理",
                                     //                Url = "/Department/Index",
                                     //            },

                                     //            new MenuViewItemModel
                                     //            {
                                     //                Icon = "icon icon-setting micon",
                                     //                IsActive = false,
                                     //                MenuType = MenuType.RoleSetting,
                                     //                Name = "角色管理",
                                     //                Url = "/Role/Index",
                                     //            },

                                     //            new MenuViewItemModel
                                     //            {
                                     //                Icon = "icon icon-setting micon",
                                     //                IsActive = false,
                                     //                MenuType = MenuType.UserRoleSetting,
                                     //                Name = "用户管理",
                                     //                Url = "/User/Index",
                                     //            },

                                     //            // new MenuViewItemModel
                                     //            //{
                                     //            //    Icon = "icon icon-setting micon",
                                     //            //    IsActive = false,
                                     //            //    MenuType = MenuType.AuthSetting,
                                     //            //    Name = "权限管理",
                                     //            //    Url = "/Auth/Index",
                                     //            //}
                                              //}
                                         //}
                                 };
                }

                return _menus;
            }

        }
    }
}