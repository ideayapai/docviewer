using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Services.Contracts
{
    [DataContract]
    [KnownType(typeof(CompositeMenuItemContract))]
    public class MenuItemContract : ContractBase, IComparable<MenuItemContract>
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [DataMember(Name = "roleId")]
        public Guid RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [DataMember(Name = "roleName")]
        public string RoleName { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember(Name = "name")]
        [Required(ErrorMessage = "名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// url地址
        /// </summary>
        [DataMember(Name = "action")]
        public string Action { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        [DataMember(Name = "menuId")]
        [Required(ErrorMessage = "ID不能为空")]
        public Guid MenuId { get; set; }
        
        /// <summary>
        /// 父级Id
        /// </summary>
        [DataMember(Name = "parentId")]
        public Guid? ParentMenuId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember(Name = "orderBy")]
        public int OrderBy { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [DataMember(Name = "category")]
        [Required(ErrorMessage = "分类不能为空")]
        public string Category { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        [DataMember(Name = "attributes")]
        public MenuItemAttributes Attributes { get; set; }

        [DataMember(Name = "children")]
        public virtual List<MenuItemContract> Items { get; set; }

        [DataMember(Name = "checked")]
        public bool IsChecked { get; set; }

        public int CompareTo(MenuItemContract other)
        {
            return OrderBy - other.OrderBy;
        }
    }

    [DataContract]
    public class CompositeMenuItemContract : MenuItemContract
    {
        private List<MenuItemContract> _items = new List<MenuItemContract>();

        [DataMember(Name = "children")]
        public override List<MenuItemContract> Items
        {
            get { return _items; }
            set { _items = value; }
        }
    }

      [DataContract]
    public class MenuItemAttributes
    {
          [DataMember(Order = 1)]
          public string Class { get; set; }

          [DataMember(Order = 2)]
          public string Id { get; set; }

          [DataMember(Order = 3)]
          public string ShowInMenu { get; set; }
    }
}
