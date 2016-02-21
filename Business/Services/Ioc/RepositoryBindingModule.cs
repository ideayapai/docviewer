using Ninject.Modules;
using Repository;
using Repository.Business;
using Repository.SqlServer;

namespace Services.Ioc
{
    public class RepositoryBindingModule : NinjectModule
    { 
        public override void Load()
        {
            Bind<IBaseRepository<Document>>().To<SQLBaseRepository<Document, DocViewerRepositoryContext>>();
            Bind<IBaseRepository<Space>>().To<SQLBaseRepository<Space, DocViewerRepositoryContext>>();
            Bind<IBaseRepository<BASE_GEO_INFO>>().To<SQLBaseRepository<BASE_GEO_INFO, DocViewerRepositoryContext>>();
            Bind<IBaseRepository<BASE_DEPARTMENT_INFO>>().To<SQLBaseRepository<BASE_DEPARTMENT_INFO, DocViewerRepositoryContext>>();
            Bind<IBaseRepository<V_DEPT_DEPT>>().To<SQLBaseRepository<V_DEPT_DEPT, DocViewerRepositoryContext>>();
            Bind<IBaseRepository<V_USER_DEPT>>().To<SQLBaseRepository<V_USER_DEPT, DocViewerRepositoryContext>>();
            Bind<IBaseRepository<V_USER_ROLE_DEPT>>().To<SQLBaseRepository<V_USER_ROLE_DEPT, DocViewerRepositoryContext>>();
            Bind<IBaseRepository<V_ROLE_FUNCTION>>().To<SQLBaseRepository<V_ROLE_FUNCTION, DocViewerRepositoryContext>>();
            Bind<IBaseRepository<BASE_FUNCTIONS>>().To<SQLBaseRepository<BASE_FUNCTIONS, DocViewerRepositoryContext>>();
            Bind<IBaseRepository<BASE_USER_INFO>>().To<SQLBaseRepository<BASE_USER_INFO, DocViewerRepositoryContext>>();
            Bind<IBaseRepository<BASE_SELECTE_VALUE_INFO>>().To<SQLBaseRepository<BASE_SELECTE_VALUE_INFO, DocViewerRepositoryContext>>();
            Bind<IBaseRepository<BASE_ROLE_INFO>>().To<SQLBaseRepository<BASE_ROLE_INFO, DocViewerRepositoryContext>>();
            Bind<IDataAuthorizeByDepRepository<V_DEPT_DEPT>>().To<DataAuthorizeByDepRepository<V_DEPT_DEPT, DocViewerRepositoryContext>>();
            Bind<IDataAuthorizeByDepRepository<V_USER_ROLE_DEPT>>().To<DataAuthorizeByDepRepository<V_USER_ROLE_DEPT, DocViewerRepositoryContext>>();
            Bind<IDataAuthorizeByDepRepository<BASE_USER_INFO>>().To<DataAuthorizeByDepRepository<BASE_USER_INFO, DocViewerRepositoryContext>>();
            Bind<IDataAuthorizeRepository<BASE_ROLE_INFO>>().To<DataAuthorizeRepository<BASE_ROLE_INFO, DocViewerRepositoryContext>>();


        }
        
    }
}