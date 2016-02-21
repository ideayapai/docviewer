using System.Configuration;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using Repository.Base;
using Repository.SqlServer;

namespace Repository.Business
{
    public class DocViewerRepositoryContext : RepositoryContext
    {
        public DocViewerRepositoryContext()
            : base(ConfigurationManager.ConnectionStrings["SQLServerConnectionString"].ConnectionString)
        {

        }

        [Function(Name = "dbo.GetUsersByUserName", IsComposable = true)]
        public IQueryable<GetUsersByUserNameResult> GetUsersByUserName([Parameter(DbType = "VarChar(32)")] string userName)
        {
            return this.CreateMethodCallQuery<GetUsersByUserNameResult>(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userName);
        }


        [Function(Name = "dbo.up_getUsersByUserName")]
        public ISingleResult<up_getUsersByUserNameResult> up_getUsersByUserName([Parameter(DbType = "VarChar(32)")] string userName)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userName);
            return ((ISingleResult<up_getUsersByUserNameResult>)(result.ReturnValue));
        }

        [Function(Name = "dbo.up_getDepsByUserName")]
        public ISingleResult<up_getDepsByUserNameResult> up_getDepsByUserName([Parameter(DbType = "VarChar(32)")] string userName)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userName);
            return ((ISingleResult<up_getDepsByUserNameResult>)(result.ReturnValue));
        }

        [Function(Name = "dbo.GetDepsByUserName", IsComposable = true)]
        public IQueryable<GetDepsByUserNameResult> GetDepsByUserName([Parameter(DbType = "VarChar(32)")] string userName)
        {
            return this.CreateMethodCallQuery<GetDepsByUserNameResult>(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userName);
        }

    }
}
