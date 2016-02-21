using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrasturcture.QueryableExtension;
using Repository;
using Services.Common;
using Services.Contracts;
using Services.Enums;

namespace Services.Area
{
    public class AreaService : IServiceProvider<AreaContract>
    {
        public readonly IBaseRepository<BASE_GEO_INFO> _areaRepository;

        public AreaService(IBaseRepository<BASE_GEO_INFO> areaRepository)
        {
            _areaRepository = areaRepository;
        }

        public PaginationList<AreaContract> Get(ExpressionCriteriaBase[] criteria, int page, int pageSize, string sort,
            string orderBy = "desc")
        {
            Expression<Func<BASE_GEO_INFO, bool>> condition = ExpressionUtils.BuildCondition<BASE_GEO_INFO>(criteria);
            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<AreaContract, BASE_GEO_INFO>(sort);

            var pager = _areaRepository.GetPager(condition, sortMap ?? "GEO_INFO_ORDER", orderBy ?? "desc",
                Math.Max(page - 1, 0) * pageSize, pageSize);
            return new PaginationList<AreaContract>(pager.Items.ConvertAll(p => p.ToContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }
        public PaginationList<AreaContract> Get(string searchString, int page, int pageSize, string sort,
            string orderBy = "desc")
        {
            Expression<Func<BASE_GEO_INFO, bool>> condition = p => p.GEO_INFO_ISDEL == 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                condition =
                    (p => p.GEO_INFO_ISDEL == 0 && (p.GEO_INFO_NAME.Contains(searchString) || p.GEO_INFO_CODE.Contains(searchString)));
            }

            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<AreaContract, BASE_GEO_INFO>(sort);

            var pager = _areaRepository.GetPager(condition, sortMap ?? "GEO_INFO_ORDER", orderBy ?? "desc",
                Math.Max(page - 1, 0) * pageSize, pageSize);
            return new PaginationList<AreaContract>(pager.Items.ConvertAll(p => p.ToContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }

        public PaginationList<AreaContract> Get(ExpressionCriteriaBase[] criteria, string[] properties, int page, int pageSize, string sort,
            string orderBy = "desc")
        {
            throw new NotImplementedException();
        }


        public AreaContract Get(Guid id)
        {
            return _areaRepository.Get(p => p.GEO_INFO_ID == id && p.GEO_INFO_ISDEL == 0).ToContract();
        }

        public AreaContract GetByName(string name)
        {
            return _areaRepository.Get(p => p.GEO_INFO_NAME.Contains(name) && p.GEO_INFO_ISDEL == 0).ToContract();
        }

        public List<AreaContract> Get(ExpressionCriteriaBase[] criteria = null, string[] properties = null)
        {
            Expression<Func<BASE_GEO_INFO, bool>> condition = ExpressionUtils.BuildCondition<BASE_GEO_INFO>(criteria);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<BASE_GEO_INFO, AreaContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<BASE_GEO_INFO, AreaContract>(properties);
                return _areaRepository.Get(selector, condition);
            }
            return _areaRepository.GetAll(condition, p => p.GEO_INFO_ORDER).ConvertAll(p => p.ToContract());

        }


        public int Count(ExpressionCriteriaBase[] criteria)
        {
            Expression<Func<BASE_GEO_INFO, bool>> condition = ExpressionUtils.BuildCondition<BASE_GEO_INFO>(criteria);
            return _areaRepository.Count(condition);
        }

        public ResultStatus Create(AreaContract item)
        {
            if (item.IsValid())
            {
                var count = _areaRepository.Count(p => p.GEO_INFO_NAME == item.AreaName && p.GEO_INFO_ISDEL == 0);
                if (count == 0)
                {
                    var entity = item.ToEntity();
                    _areaRepository.Add(entity);
                    return ResultStatus.Success;
                }
                return ResultStatus.Duplicate;
            }
            return ResultStatus.Failed;
        }

        public ResultStatus Update(AreaContract item)
        {
            if (item.IsValid())
            {
                _areaRepository.Update(p => p.GEO_INFO_ID == item.Id, u =>
                                                                 {
                                                                     u.GEO_INFO_NAME = item.AreaName;
                                                                     u.GEO_INFO_PARENT = item.ParentId;
                                                                     u.GEO_INFO_CODE = item.AreaNo;
                                                                 });
                return ResultStatus.Success;
            }
            return ResultStatus.Failed;
        }

        public bool Delete(List<Guid> ids)
        {
            if (ids.Count > 0)
            {
                return _areaRepository.Delete(p => ids.Contains(p.GEO_INFO_ID));
            }
            return false;
        }

        public bool Delete(Guid id)
        {
            return
                _areaRepository.Delete(p => id == p.GEO_INFO_ID);
        }

        public bool SetDelete(Guid id)
        {
            BASE_GEO_INFO areaInfo = _areaRepository.Update(u => u.GEO_INFO_ID == id, (u) =>
            {
                u.GEO_INFO_ISDEL = 1;
            });
            if (areaInfo != null)

                return true;
            else
                return false;
        }

        public bool IsExist(Guid id, string areaName)
        {
            return _areaRepository.Count(p => p.GEO_INFO_NAME == areaName && p.GEO_INFO_ID != id && p.GEO_INFO_ISDEL == 0) != 0;
        }

    }
}
