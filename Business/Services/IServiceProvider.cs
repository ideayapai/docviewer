using System;
using System.Collections.Generic;
using Infrasturcture.QueryableExtension;
using Repository;
using Services.Enums;

namespace Services
{
    public interface IServiceProvider<T> where T : ContractBase
    {
        PaginationList<T> Get(ExpressionCriteriaBase[] criteria, int page, int pageSize, string sort, string orderBy = "desc");

        PaginationList<T> Get(ExpressionCriteriaBase[] criteria, string[] properties, int page, int pageSize, string sort, string orderBy = "desc");


        T Get(Guid id);

        List<T> Get(ExpressionCriteriaBase[] criteria = null, string[] properties = null);

        int Count(ExpressionCriteriaBase[] criteria);


        ResultStatus Create(T item);


        ResultStatus Update(T item);

        bool Delete(List<Guid> ids);

        bool Delete(Guid id);

        bool IsExist(Guid id, string name);
    }
}
