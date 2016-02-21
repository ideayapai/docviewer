using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrasturcture.QueryableExtension;
using Repository;
using Services.Common;
using Services.Contracts;
using Services.Enums;

namespace Services
{
    public class SelectValueService
    {
        private readonly IBaseRepository<BASE_SELECTE_VALUE_INFO> _selectBaseRepository;

        public SelectValueService(IBaseRepository<BASE_SELECTE_VALUE_INFO> selectBaseRepository)
        {
            _selectBaseRepository = selectBaseRepository;
        }

        public List<SelectValueContract> Get(string belong, string[] properties = null)
        {
            Expression<Func<BASE_SELECTE_VALUE_INFO, bool>> condition = p => true;

            if (!string.IsNullOrEmpty(belong))
            {
                condition =
                    (p => p.Belong.ToLower() == belong.ToLower());
            }

            if (properties != null && properties.Length > 0)
            {
                properties = new[] { "Id", "Name", "Code", "SOrder" };
                Expression<Func<BASE_SELECTE_VALUE_INFO, SelectValueContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<BASE_SELECTE_VALUE_INFO, SelectValueContract>(properties);
                return _selectBaseRepository.Get(selector, condition).OrderBy(p => p.SOrder).ToList();
            }
            return _selectBaseRepository.GetAll(condition, p => p.SOrder).OrderBy(p => p.SOrder).ToList().ConvertAll(p => p.ToContract());

        }

        public PaginationList<SelectValueContract> Get(string searchString, int page, int pageSize, string sort,
            string orderBy = "desc")
        {
            Expression<Func<BASE_SELECTE_VALUE_INFO, bool>> condition = p => true;

            if (!string.IsNullOrEmpty(searchString))
            {
                condition =
                    (p => true && (p.Name.Contains(searchString) || p.Belong.Contains(searchString)));
            }

            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<SelectValueContract, BASE_SELECTE_VALUE_INFO>(sort);

            var pager = _selectBaseRepository.GetPager(condition, sortMap ?? "Belong", orderBy ?? "asc",
                Math.Max(page - 1, 0) * pageSize, pageSize);

            return new PaginationList<SelectValueContract>(pager.Items.ConvertAll(p => p.ToContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }

        public SelectValueContract Get(string value)
        {
            Expression<Func<BASE_SELECTE_VALUE_INFO, bool>> condition = null;
            if (!string.IsNullOrEmpty(value))
            {
                condition =
                    p => p.Id == Guid.Parse(value);
            }
            return _selectBaseRepository.Get(condition).ToContract();

        }

        public List<SelectValueContract> Get(string[] value)
        {
            Expression<Func<BASE_SELECTE_VALUE_INFO, bool>> condition = null;
            var ids = value.Select(Guid.Parse).ToList();
            condition =
                 p => ids.Contains(p.Id);
            return _selectBaseRepository.GetAll(condition).ConvertAll(p => p.ToContract());

        }

        public List<SelectValueContract> GetContains(string belong, string[] properties = null)
        {
            Expression<Func<BASE_SELECTE_VALUE_INFO, bool>> condition = null;

            if (!string.IsNullOrEmpty(belong))
            {
                condition =
                    (p => p.Belong.ToLower().Contains(belong.ToLower()));
            }

            if (properties != null && properties.Length > 0)
            {
                properties = new[] { "Id", "Name", "Code", "SOrder" };
                Expression<Func<BASE_SELECTE_VALUE_INFO, SelectValueContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<BASE_SELECTE_VALUE_INFO, SelectValueContract>(properties);
                return _selectBaseRepository.Get(selector, condition).OrderBy(p => p.SOrder).ToList();
            }
            return _selectBaseRepository.GetAll(condition, p => p.SOrder).OrderBy(p => p.SOrder).ToList().ConvertAll(p => p.ToContract());

        }

        public bool IsExist(string id)
        {
            Expression<Func<BASE_SELECTE_VALUE_INFO, bool>> condition = null;
            if (!string.IsNullOrEmpty(id))
            {
                condition =
                    p => p.Id == Guid.Parse(id);
            }
            return _selectBaseRepository.Count(condition) > 0;
        }

        public ResultStatus Create(SelectValueContract item)
        {
            if (item.IsValid())
            {
                var count = _selectBaseRepository.Count(p => p.Belong == item.Belong && (p.Name == item.Name || p.Code == item.Code));
                if (count == 0)
                {
                    var entity = item.ToEntity();
                    _selectBaseRepository.Add(entity);
                    return ResultStatus.Success;
                }
                return ResultStatus.Duplicate;
            }
            return ResultStatus.Failed;
        }

        public bool UpdateName(Guid id, string name)
        {

            BASE_SELECTE_VALUE_INFO cc = _selectBaseRepository.Update(u => u.Id == id, (u) =>
            {
                u.Name = name;
            });
            if (cc != null)

                return true;
            else
                return false;
        }
    }
}
