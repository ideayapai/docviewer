using System.Collections.Generic;
using Infrasturcture.QueryableExtension;
using Repository;

namespace Services
{
    interface IDataAuthorizeByDepServiceProvider<T> where T : ContractBase
    {

        PaginationList<T> Get(string userName, ExpressionCriteriaBase[] criteria, int page, int pageSize, string sort, string orderBy = "desc");

        PaginationList<T> Get(string userName, ExpressionCriteriaBase[] criteria, string[] properties, int page, int pageSize, string sort, string orderBy = "desc");

        List<T> Get(string userName, ExpressionCriteriaBase[] criteria = null, string[] properties = null);

        int Count(string userName, ExpressionCriteriaBase[] criteria);
    }
}
