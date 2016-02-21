using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrasturcture.QueryableExtension;
using Repository;
using Repository.Business;
using Services.Common;
using Services.Contracts;
using Services.Enums;
using Services.User;

namespace Services.Department
{
    public class DepartmentService : IDataAuthorizeByDepServiceProvider<DepartmentContract>, IServiceProvider<DepartmentContract>
    {
        private readonly IBaseRepository<BASE_DEPARTMENT_INFO> _departmentRepository;
        private readonly IDataAuthorizeByDepRepository<V_DEPT_DEPT> _depDepAuthorizeRepository;
        private readonly IBaseRepository<V_DEPT_DEPT> _depDepRepository;
        private readonly UserService _userService;

        public DepartmentService(IBaseRepository<BASE_DEPARTMENT_INFO> departmentRepository, IBaseRepository<V_DEPT_DEPT> depDepRepository,
            IDataAuthorizeByDepRepository<V_DEPT_DEPT> depDepAuthorizeRepository, UserService userService)
        {
            _departmentRepository = departmentRepository;
            _depDepRepository = depDepRepository;
            _depDepAuthorizeRepository = depDepAuthorizeRepository;
            _userService = userService;
        }
        /// <summary>
        /// 注册供应商并分配账号
        /// </summary>
        /// <param name="departemnt"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public int RegisterSupAndUser(DepartmentContract departemnt, UserContract user)
        {
            int result = 1;
            Guid dId = Guid.Empty;

            var rs = Register(departemnt, out dId);
            if (rs != ResultStatus.Success || dId == Guid.Empty)
            {
                return 2;
            }
            user.DepId = dId;

            var rt = _userService.Create(user);

            if (rt != ResultStatus.Success)
            {
                Delete(dId);
                return 3;
            }

            return result;
        }

        /// <summary>
        /// 供应商公共注册
        /// </summary>
        /// <param name="departemnt"></param>
        /// <returns></returns>
        public ResultStatus Register(DepartmentContract departemnt, out Guid id)
        {
            id = Guid.Empty;
            if (departemnt.IsValid())
            {
                departemnt.DeptInfoIssup = 1;
                var count = _departmentRepository.Count(p => p.DEPT_INFO_NAME == departemnt.Name && p.DEPT_INFO_ISDEL == 0);
                if (count == 0)
                {
                    var entity = departemnt.ToEntity();
                    id = _departmentRepository.Add(entity).DEPT_INFO_ID;
                    return ResultStatus.Success;
                }
                return ResultStatus.Duplicate;
            }
            return ResultStatus.Failed;
        }

        public int Count(string sName)
        {
            var count = _departmentRepository.Count(p => p.DEPT_INFO_NAME == sName && p.DEPT_INFO_ISDEL == 0);
            return count;
        }


        public ResultStatus Create(DepartmentContract departemnt)
        {
            if (departemnt.IsValid())
            {
                var count = _departmentRepository.Count(p => p.DEPT_INFO_NAME == departemnt.Name && p.DEPT_INFO_ISDEL == 0);
                if (count == 0)
                {
                    departemnt.CreatedTime = DateTime.Now;
                    var entity = departemnt.ToEntity();
                    _departmentRepository.Add(entity);
                    return ResultStatus.Success;
                }
                return ResultStatus.Duplicate;
            }
            return ResultStatus.Failed;
        }

        public ResultStatus Update(DepartmentContract item)
        {
            if (item.IsValid())
            {
                var count = _departmentRepository.Count(p => p.DEPT_INFO_ID != item.Id && p.DEPT_INFO_NAME == item.Name && p.DEPT_INFO_ISDEL == 0);
                if (count == 0)
                {
                    item.CreatedTime = DateTime.Now;
                    _departmentRepository.Update(p => p.DEPT_INFO_ID == item.Id && p.DEPT_INFO_ISDEL == 0, item.ToEntity());
                    return ResultStatus.Success;
                }
                return ResultStatus.Duplicate;
            }
            return ResultStatus.Failed;
        }

        public PaginationList<DepartmentContract> Get(ExpressionCriteriaBase[] criteria, int page, int pageSize, string sort, string orderBy = "desc")
        {
            Expression<Func<V_DEPT_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_DEPT_DEPT>(criteria);
            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<DepartmentContract, V_DEPT_DEPT>(sort);

            var pager = _depDepRepository.GetPager(condition, sortMap ?? "DEPT_INFO_CREATEDDATE", orderBy ?? "desc",
                Math.Max(page - 1, 0) * pageSize, pageSize);
            return new PaginationList<DepartmentContract>(pager.Items.ConvertAll(p => p.ToContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }

        public PaginationList<DepartmentContract> Get(ExpressionCriteriaBase[] criteria, string[] properties, int page, int pageSize, string sort,
            string orderBy = "desc")
        {
            Expression<Func<V_DEPT_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_DEPT_DEPT>(criteria);
            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<DepartmentContract, V_DEPT_DEPT>(sort);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<V_DEPT_DEPT, DepartmentContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<V_DEPT_DEPT, DepartmentContract>(properties);
                return _depDepRepository.GetPager(selector, condition, sortMap ?? "DEPT_INFO_CREATEDDATE", orderBy ?? "desc",
                    Math.Max(page - 1, 0) * pageSize, pageSize);
            }
            var pager = _depDepRepository.GetPager(condition, sortMap ?? "DEPT_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);

            return new PaginationList<DepartmentContract>(pager.Items.ConvertAll(p => p.ToContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }

        public PaginationList<DepartmentContract> Get(string createdBy, string searchString, string[] properties, int page, int pageSize, string sort,
            string orderBy = "desc")
        {
            Expression<Func<V_DEPT_DEPT, bool>> condition = p => p.DEPT_INFO_ISDEL == 0 && p.DEPT_INFO_ISSUP == 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                condition = (p => p.DEPT_INFO_ISDEL == 0 && p.DEPT_INFO_ISSUP == 0 && (p.DEPT_INFO_NAME.Contains(searchString) || p.DEPT_INFO_PARENTNAME.Contains(searchString) || p.DEPT_INFO_ADDRESS.Contains(searchString) || p.GEO_INFO_NAME.Contains(searchString)));
            }

            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<DepartmentContract, V_DEPT_DEPT>(sort);
            if (properties != null && properties.Length > 0)
            {
                var selector = QueryableBuilderExtensions.BuildSelectorClause<V_DEPT_DEPT, DepartmentContract>(properties);
                return _depDepAuthorizeRepository.GetPagerByUser(createdBy, selector, condition, sortMap ?? "DEPT_INFO_CREATEDDATE", orderBy ?? "desc",
                    Math.Max(page - 1, 0) * pageSize, pageSize);
            }
            var pager = _depDepAuthorizeRepository.GetPagerByUser(createdBy, condition, sortMap ?? "DEPT_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);

            return new PaginationList<DepartmentContract>(pager.Items.ConvertAll(p => p.ToContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }
    

        public DepartmentContract Get(Guid id)
        {
            return _departmentRepository.Get(p => p.DEPT_INFO_ID == id && p.DEPT_INFO_ISDEL == 0).ToContract();
        }

        public DepartmentContract GetByName(string name)
        {
            return _departmentRepository.Get(p => p.DEPT_INFO_NAME.Contains(name) && p.DEPT_INFO_ISDEL == 0).ToContract();
        }

        public DepartmentContract GetMaxTopDept()
        {
            return _departmentRepository.Get(p => p.PARENT_ID == null && p.DEPT_INFO_ISDEL == 0).ToContract();
        }

        public List<DepartmentContract> GetByUser(string createdBy, ExpressionCriteriaBase[] criteria = null, string[] properties = null)
        {
            Expression<Func<V_DEPT_DEPT, bool>> condition = (p => p.DEPT_INFO_ISDEL == 0);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<V_DEPT_DEPT, DepartmentContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<V_DEPT_DEPT, DepartmentContract>(properties);
                return _depDepAuthorizeRepository.GetByUser(createdBy, selector, condition);
            }
            return _depDepAuthorizeRepository.GetByUser(createdBy, condition, p => p.DEPT_INFO_CREATEDDATE).ConvertAll(p => p.ToContract());

        }
        /// <summary>
        /// 供应商下拉列表
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public List<DepartmentContract> GetSups(ExpressionCriteriaBase[] criteria = null, string[] properties = null)
        {
            Expression<Func<V_DEPT_DEPT, bool>> condition = (p => p.DEPT_INFO_ISDEL == 0 && p.DEPT_INFO_ISSUP == 1);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<V_DEPT_DEPT, DepartmentContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<V_DEPT_DEPT, DepartmentContract>(properties);
                return _depDepRepository.Get(selector, condition);
            }
            return _depDepRepository.GetAll(condition, p => p.DEPT_INFO_CREATEDDATE).ConvertAll(p => p.ToContract());

        }

        public List<DepartmentContract> Get(ExpressionCriteriaBase[] criteria = null, string[] properties = null)
        {
            Expression<Func<V_DEPT_DEPT, bool>> condition = (p => p.DEPT_INFO_ISDEL == 0 && p.DEPT_INFO_ISSUP == 0);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<V_DEPT_DEPT, DepartmentContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<V_DEPT_DEPT, DepartmentContract>(properties);
                return _depDepRepository.Get(selector, condition);
            }
            return _depDepRepository.GetAll(condition, p => p.DEPT_INFO_CREATEDDATE).ConvertAll(p => p.ToContract());

        }


        public int Count(ExpressionCriteriaBase[] criteria)
        {
            Expression<Func<BASE_DEPARTMENT_INFO, bool>> condition = ExpressionUtils.BuildCondition<BASE_DEPARTMENT_INFO>(criteria);
            return _departmentRepository.Count(condition);

        }

        public bool Delete(List<Guid> ids)
        {
            if (ids.Count > 0)
            {
                return _departmentRepository.Delete(p => ids.Contains(p.DEPT_INFO_ID));
            }
            return false;
        }

        public bool Delete(Guid id)
        {
            return _departmentRepository.Delete(p => id == p.DEPT_INFO_ID && p.DEPT_INFO_ISDEL == 0);
        }

        public bool SetDelete(Guid id)
        {
            BASE_DEPARTMENT_INFO departmentInfo = _departmentRepository.Update(u => u.DEPT_INFO_ID == id && u.DEPT_INFO_ISDEL == 0, (u) =>
            {
                u.DEPT_INFO_ISDEL = 1;
            });
            if (departmentInfo != null)

                return true;
            else
                return false;
        }

        public bool IsExist(Guid id, string name)
        {
            return _departmentRepository.Count(p => p.DEPT_INFO_ID != id && p.DEPT_INFO_NAME == name && p.DEPT_INFO_ISDEL == 0) > 0;
        }

        public PaginationList<DepartmentContract> Get(string createdBy, ExpressionCriteriaBase[] criteria, int page, int pageSize, string sort,
            string orderBy = "desc")
        {
            Expression<Func<V_DEPT_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_DEPT_DEPT>(criteria);
            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<DepartmentContract, V_DEPT_DEPT>(sort);

            var pager = _depDepAuthorizeRepository.GetPagerByUser(createdBy, condition, sortMap ?? "DEPT_INFO_CREATEDDATE", orderBy ?? "desc",
                Math.Max(page - 1, 0) * pageSize, pageSize);
            return new PaginationList<DepartmentContract>(pager.Items.ConvertAll(p => p.ToContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }

        public PaginationList<DepartmentContract> Get(string createdBy, ExpressionCriteriaBase[] criteria, string[] properties, int page, int pageSize,
            string sort, string orderBy = "desc")
        {
            Expression<Func<V_DEPT_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_DEPT_DEPT>(criteria);
            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<DepartmentContract, V_DEPT_DEPT>(sort);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<V_DEPT_DEPT, DepartmentContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<V_DEPT_DEPT, DepartmentContract>(properties);
                return _depDepAuthorizeRepository.GetPagerByUser(createdBy, selector, condition, sortMap ?? "DEPT_INFO_CREATEDDATE", orderBy ?? "desc",
                    Math.Max(page - 1, 0) * pageSize, pageSize);
            }
            var pager = _depDepAuthorizeRepository.GetPagerByUser(createdBy, condition, sortMap ?? "DEPT_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);

            return new PaginationList<DepartmentContract>(pager.Items.ConvertAll(p => p.ToContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }

        public List<DepartmentContract> Get(string createdBy, ExpressionCriteriaBase[] criteria = null, string[] properties = null)
        {
            Expression<Func<V_DEPT_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_DEPT_DEPT>(criteria);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<V_DEPT_DEPT, DepartmentContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<V_DEPT_DEPT, DepartmentContract>(properties);
                return _depDepAuthorizeRepository.GetByUser(createdBy, selector, condition).ToList();
            }
            return _depDepAuthorizeRepository.GetByUser(createdBy, condition, p => p.DEPT_INFO_CREATEDDATE).ConvertAll(p => p.ToContract());
        }

        public int Count(string createdBy, ExpressionCriteriaBase[] criteria)
        {
            Expression<Func<V_DEPT_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_DEPT_DEPT>(criteria);
            return _depDepAuthorizeRepository.CountByUser(createdBy, condition);
        }
    }
}
