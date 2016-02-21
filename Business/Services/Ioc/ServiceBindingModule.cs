using Infrasturcture.Store;
using Ninject.Modules;
using Search;
using Services.Area;
using Services.CacheService;
using Services.Department;
using Services.Documents;
using Services.Role;
using Services.Search;
using Services.Search.UnitOfWork;
using Services.Spaces;
using Services.User;

namespace Services.Ioc
{
    public class ServiceBindingModule : NinjectModule
    { 
        public override void Load()
        {
     
            Bind<DocumentService>().ToSelf().InSingletonScope();
            Bind<DocumentConvertService>().ToSelf().InSingletonScope();
            Bind<SpaceService>().ToSelf().InSingletonScope();
            Bind<SpaceTreeService>().ToSelf().InSingletonScope();
            Bind<SearchService>().ToSelf().InSingletonScope();
            Bind<DocumentCacheService>().ToSelf().InSingletonScope();
            Bind<DocumentListCacheService>().ToSelf().InSingletonScope();
            Bind<SpaceCacheService>().ToSelf().InSingletonScope();
            Bind<SpaceListCacheService>().ToSelf().InSingletonScope();
            Bind<AreaService>().ToSelf().InSingletonScope();
            Bind<DepartmentService>().ToSelf().InSingletonScope();
            Bind<SelectValueService>().ToSelf().InSingletonScope();
            Bind<RoleService>().ToSelf().InSingletonScope();
            Bind<RoleFunctionService>().ToSelf().InSingletonScope();
            Bind<UserService>().ToSelf().InSingletonScope();
            Bind<AddSpaceIndexerxUnitOfWork>().ToSelf().InSingletonScope();
            Bind<AddDocumentIndexerxUnitOfWork>().ToSelf().InSingletonScope();
            Bind<RemoveSpaceIndexerxUnitOfWork>().ToSelf().InSingletonScope();
            Bind<RemoveDocumentIndexerxUnitOfWork>().ToSelf().InSingletonScope();
            Bind<IFileContentReader>().To<FileContentReader>().InSingletonScope();

        }
        
    }
}