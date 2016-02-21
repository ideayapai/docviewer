using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrasturcture.QueryableExtension;
using Repository;
using Repository.Business;
using Services.Common;
using Services.Contracts;
using Services.Enums;

namespace Services.Role
{
    public class RoleService : IServiceProvider<RoleContract>
    {
        private readonly IBaseRepository<BASE_ROLE_INFO> _roleRepository;
        private readonly IDataAuthorizeRepository<BASE_ROLE_INFO> _roleAuthorizeRepository;

        public RoleService(IBaseRepository<BASE_ROLE_INFO> roleRepository, IDataAuthorizeRepository<BASE_ROLE_INFO> roleAuthorizeRepository)
        {
            _roleRepository = roleRepository;
            _roleAuthorizeRepository = roleAuthorizeRepository;
        }

        public PaginationList<RoleContract> Get(ExpressionCriteriaBase[] criteria, int page, int pageSize, string sort,
            string orderBy = "desc")
        {
            Expression<Func<BASE_ROLE_INFO, bool>> condition = ExpressionUtils.BuildCondition<BASE_ROLE_INFO>(criteria);
            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<RoleContract, BASE_ROLE_INFO>(sort);

            var pager = _roleRepository.GetPager(condition, sortMap ?? "ROLE_INFO_CREATEDDATE", orderBy ?? "desc",
                Math.Max(page - 1, 0) * pageSize, pageSize);
            return new PaginationList<RoleContract>(pager.Items.ConvertAll(p => p.ToContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }
        public PaginationList<RoleContract> Get(string searchString, int page, int pageSize, string sort,
            string orderBy = "desc")
        {
            Expression<Func<BASE_ROLE_INFO, bool>> condition = p => p.IsDel == 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                condition =
                    (p => p.IsDel == 0 && (p.ROLE_INFO_NAME.Contains(searchString)));
            }

            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<RoleContract, BASE_ROLE_INFO>(sort);

            var pager = _roleRepository.GetPager(condition, sortMap ?? "ROLE_INFO_CREATEDDATE", orderBy ?? "desc",
                Math.Max(page - 1, 0) * pageSize, pageSize);
            return new PaginationList<RoleContract>(pager.Items.ConvertAll(p => p.ToContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }

        public PaginationList<RoleContract> GetByUser(string createdBy, string searchString, int page, int pageSize, string sort,
            string orderBy = "desc")
        {
            Expression<Func<BASE_ROLE_INFO, bool>> condition = p => p.IsDel == 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                condition =
                    (p => p.IsDel == 0 && (p.ROLE_INFO_NAME.Contains(searchString)));
            }

            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<RoleContract, BASE_ROLE_INFO>(sort);

            var pager = _roleAuthorizeRepository.GetPagerByUser(createdBy, condition, sortMap ?? "ROLE_INFO_CREATEDDATE", orderBy ?? "desc",
                Math.Max(page - 1, 0) * pageSize, pageSize);
            return new PaginationList<RoleContract>(pager.Items.ConvertAll(p => p.ToContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }

        public PaginationList<RoleContract> Get(ExpressionCriteriaBase[] criteria, string[] properties, int page, int pageSize, string sort,
            string orderBy = "desc")
        {
            throw new NotImplementedException();
        }


        public RoleContract Get(Guid id)
        {
            return _roleRepository.Get(p => p.ROLE_INFO_ID == id).ToContract();
        }

        public List<RoleContract> Get(ExpressionCriteriaBase[] criteria = null, string[] properties = null)
        {
            Expression<Func<BASE_ROLE_INFO, bool>> condition = ExpressionUtils.BuildCondition<BASE_ROLE_INFO>(criteria);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<BASE_ROLE_INFO, RoleContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<BASE_ROLE_INFO, RoleContract>(properties);
                return _roleRepository.Get(selector, condition);
            }
            return _roleRepository.GetAll(condition, p => p.ROLE_INFO_CREATEDDATE).ConvertAll(p => p.ToContract());

        }
        public List<RoleContract> GetByUser(string createBy, ExpressionCriteriaBase[] criteria = null, string[] properties = null)
        {
            Expression<Func<BASE_ROLE_INFO, bool>> condition = ExpressionUtils.BuildCondition<BASE_ROLE_INFO>(criteria);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<BASE_ROLE_INFO, RoleContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<BASE_ROLE_INFO, RoleContract>(properties);
                return _roleAuthorizeRepository.GetByUser(createBy, selector, condition);
            }
            return _roleAuthorizeRepository.GetByUser(createBy, condition, p => p.ROLE_INFO_CREATEDDATE).ConvertAll(p => p.ToContract());

        }

        public int Count(ExpressionCriteriaBase[] criteria)
        {
            Expression<Func<BASE_ROLE_INFO, bool>> condition = ExpressionUtils.BuildCondition<BASE_ROLE_INFO>(criteria);
            return _roleRepository.Count(condition);
        }

        public ResultStatus Create(RoleContract item)
        {
            if (item.IsValid())
            {
                var count = _roleRepository.Count(p => p.ROLE_INFO_NAME == item.Name);
                if (count == 0)
                {
                    item.CreatedDate = DateTime.Now;
                    var entity = item.ToEntity();
                    _roleRepository.Add(entity);
                    return ResultStatus.Success;
                }
                return ResultStatus.Duplicate;
            }
            return ResultStatus.Failed;
        }

        public ResultStatus Update(RoleContract role)
        {
            if (role.IsValid())
            {
                _roleRepository.Update(p => p.ROLE_INFO_ID == role.Id, u =>
                {
                    u.ROLE_INFO_NAME = role.Name;
                    u.ROLE_INFO_CREATEDDATE = DateTime.Now;
                });
                return ResultStatus.Success;
            }
            return ResultStatus.Failed;
        }

        public bool Delete(List<Guid> ids)
        {
            if (ids.Count > 0)
            {
                return _roleRepository.Delete(p => ids.Contains(p.ROLE_INFO_ID) && p.ROLE_INFO_TYPE.ToLower() != ServiceConstants.SystemRoleType);
            }
            return false;
        }

        public bool Delete(Guid id)
        {
            return
                _roleRepository.Delete(
                    p => id == p.ROLE_INFO_ID && p.ROLE_INFO_TYPE.ToLower() != ServiceConstants.SystemRoleType);
        }

        public bool SetDelete(Guid id)
        {
            BASE_ROLE_INFO user = _roleRepository.Update(u => u.ROLE_INFO_ID == id && u.ROLE_INFO_TYPE.ToLower() != ServiceConstants.SystemRoleType, (u) =>
            {
                u.IsDel = 1;
            });
            if (user != null)

                return true;
            else
                return false;
        }

        public bool IsExist(Guid id, string roleName)
        {
            return _roleRepository.Count(p => p.ROLE_INFO_NAME == roleName && p.ROLE_INFO_ID != id) != 0;
        }

        public RoleContract GetMaxTop(string sql)
        {
            return _roleRepository.Get(p => p.IsDel == 0 && p.ROLE_INFO_NAME.Contains(sql)).ToContract();
        }
    }
}
