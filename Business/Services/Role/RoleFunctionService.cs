using System;
using System.Collections.Generic;
using System.Linq;
using Repository;
using Repository.Business;
using Services.Common;
using Services.Contracts;
using Services.Enums;

namespace Services.Role
{
    public class RoleFunctionService
    {
        private readonly IBaseRepository<V_ROLE_FUNCTION> _roleFunctionsViewRepository;
        private readonly IBaseRepository<BASE_FUNCTIONS> _functionsRepository;

        public RoleFunctionService(IBaseRepository<V_ROLE_FUNCTION> roleFunctionsViewRepository, IBaseRepository<BASE_FUNCTIONS> functionsRepository)
        {
            _roleFunctionsViewRepository = roleFunctionsViewRepository;
            _functionsRepository = functionsRepository;
        }

        public IList<MenuItemContract> GetFunctions()
        {
            var roleFunctions = _functionsRepository.GetAll(p => true).ConvertAll(p => p.ToContract());

            return roleFunctions.Distinct().OrderBy(p => p.Category).ThenBy(p => p.OrderBy).ToList();
        }

        public MenuItemContract Get(Guid roleId, string category)
        {
            var roleFunctions =
                _roleFunctionsViewRepository.GetAll(p => p.ROLE_INFO_ID == roleId && p.FUNCTIONS_CATEGORY == category).ConvertAll(
                    p => p.ToContract());
            var rs = BuildFunctionTree(roleFunctions);
            if (rs.Count > 0)
                return rs[0];
            return new MenuItemContract();
        }

        public MenuItemContract GetById(Guid Id)
        {
            return _functionsRepository.Get(p => p.FUNCTIONS_ID == Id).ToContract();
        }

        public IList<MenuItemContract> GetByRole(Guid roleId)
        {
            var roleFunctions =
                _roleFunctionsViewRepository.GetAll(p => p.ROLE_INFO_ID == roleId).ConvertAll(
                    p => p.ToContract());

            return roleFunctions;
        }
        /// <summary>
        /// 左侧菜单展示ShowInMenu：1展示，0：不展示
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IList<MenuItemContract> Get(Guid roleId)
        {
            return Get(roleId, false);
        }

        private IList<MenuItemContract> GetAll()
        {
            var roleFunctions = _functionsRepository.GetAll(p => true).ConvertAll(p => p.ToContract());
            return roleFunctions;
        }

        public IList<MenuItemContract> Get(Guid roleId, bool showAll)
        {
            var roleFunctions = _roleFunctionsViewRepository.GetAll(p => p.ROLE_INFO_ID == roleId).ConvertAll(p => p.ToContract()).FindAll(p => p.Attributes != null && (string.IsNullOrEmpty(p.Attributes.ShowInMenu) || p.Attributes.ShowInMenu == "1"));
            if (showAll)
            {
                var allFunctions = _functionsRepository.GetAll(p => true).ConvertAll(p => p.ToContract()).FindAll(p => p.Attributes != null && (string.IsNullOrEmpty(p.Attributes.ShowInMenu) || p.Attributes.ShowInMenu == "1"));
                foreach (var function in roleFunctions)
                {
                    allFunctions.Find(p => p.MenuId == function.MenuId).IsChecked = true;
                }
                return BuildFunctionTree(allFunctions);
            }
            return BuildFunctionTree(roleFunctions);
        }

        public void Update(Guid roleId, List<Guid> functionsId)
        {

            var all = GetAll();
            var functionIds = new List<Guid>();
            foreach (var id in functionsId)
            {
                var temp = id;
            loop1:
                var item = all.First(p => p.MenuId == temp);
                functionIds.Add(item.MenuId);
                if (item.ParentMenuId.HasValue)
                {
                    temp = item.ParentMenuId.Value;
                    goto loop1;
                }
            }
            var roleFunctions = functionIds.Distinct().Select(p => new RoleFunctionContract
            {
                RoleId = roleId,
                FunctionId = p,
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
            }).ToList();
            using (var dc = new DocViewerRepositoryContext())
            {
                var entities = dc.GetTable<BASE_ROLEFUNCTIONS>().Where(p => p.ROLE_INFO_ID == roleId);
                dc.GetTable<BASE_ROLEFUNCTIONS>().DeleteAllOnSubmit(entities);
                if (roleFunctions.Count > 0)
                    dc.GetTable<BASE_ROLEFUNCTIONS>().InsertAllOnSubmit(roleFunctions.ConvertAll(p => p.ToEntity()));
                dc.SubmitChanges();
            }
        }

        public IList<ShortMenuItem> GetShortMenu(Guid roleId, bool showAll)
        {
            IList<MenuItemContract> funcitons = null;
            var roleFunctions = _roleFunctionsViewRepository.GetAll(p => p.ROLE_INFO_ID == roleId).ConvertAll(p => p.ToContract());
            if (showAll)
            {
                var allFunctions = _functionsRepository.GetAll(p => true).ConvertAll(p => p.ToContract());
                foreach (var function in roleFunctions)
                {
                    allFunctions.Find(p => p.MenuId == function.MenuId).IsChecked = true;
                }
                funcitons = BuildFunctionTree(allFunctions);
            }
            else
            {
                funcitons = BuildFunctionTree(roleFunctions);

            }
            var menus = new List<ShortMenuItem>(funcitons.Count);
            menus.AddRange(funcitons.Select(Build));
            return menus;
        }

        ShortMenuItem Build(MenuItemContract source)
        {
            if (source is CompositeMenuItemContract)
            {
                var shortItem = new ShortMenuItem
                {
                    Id = source.MenuId,
                    Children = new List<ShortMenuItem>(),
                    IsChecked = source.IsChecked,
                    Name = source.Name,
                    Title = source.Name + "(" + source.Action + ")",
                    ParentId = source.ParentMenuId,
                };
                foreach (var item in source.Items)
                {
                    shortItem.Children.Add(Build(item));
                }
                if (shortItem.Children != null && shortItem.Children.All(p => p.IsChecked))
                {
                    shortItem.IsChecked = true;
                }
                return shortItem;
            }
            else
            {
                return new ShortMenuItem
                {
                    Id = source.MenuId,
                    IsChecked = source.IsChecked,
                    Name = source.Name,
                    Title = source.Name + "(" + source.Action + ")",
                    ParentId = source.ParentMenuId,
                };
            }
        }

        private IList<MenuItemContract> BuildFunctionTree(IList<MenuItemContract> source)
        {
            var roots = source.Where(p => !p.ParentMenuId.HasValue).ToList();
            roots.Sort();
            IList<MenuItemContract> rs = new List<MenuItemContract>(Math.Max(roots.Count, 1));
            foreach (var item in roots)
            {
                rs.Add(Walk(source, item));
            }
            return rs;
        }

        private MenuItemContract Walk(IList<MenuItemContract> source, MenuItemContract current)
        {
            var children = source.Where(p => p.ParentMenuId == current.MenuId).ToList();
            if (children.Count > 0)
            {
                var item = new CompositeMenuItemContract()
                {
                    RoleId = current.RoleId,
                    RoleName = current.RoleName,
                    Name = current.Name,
                    Action = current.Action,
                    MenuId = current.MenuId,
                    ParentMenuId = current.ParentMenuId,
                    OrderBy = current.OrderBy,
                    Category = current.Category,
                    Attributes = current.Attributes
                };
                foreach (var child in children)
                {
                    item.Items.Add(Walk(source, child));
                }
                item.Items.Sort();
                return item;
            }
            return current;
        }

        public ResultStatus Create(MenuItemContract item)
        {
            if (item.IsValid())
            {
                var count = _functionsRepository.Count(p => p.FUNCTIONS_ID == item.MenuId);
                if (count == 0)
                {
                    var entity = item.ToEntity();
                    _functionsRepository.Add(entity);
                    return ResultStatus.Success;
                }
                return ResultStatus.Duplicate;
            }
            return ResultStatus.Failed;
        }

        public ResultStatus Update(MenuItemContract item)
        {
            if (item.IsValid())
            {
                _functionsRepository.Update(p => p.FUNCTIONS_ID == item.MenuId, item.ToEntity());
                return ResultStatus.Success;
            }
            return ResultStatus.Failed;
        }
    }
}
