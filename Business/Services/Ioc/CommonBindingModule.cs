using Documents.Converter;
using Infrasturcture.Cache;
using Infrasturcture.Store;
using Infrasturcture.Store.Mongo;
using Infrasturcture.Web;
using Messages;
using Ninject.Modules;
using Search;
using Services.Context;
using Services.Search;

namespace Services.Ioc
{
    public class CommonBindingModule : NinjectModule
    { 
        public override void Load()
        {
            Bind<IHttpRequestProvider>().To<DefaultRequestProvider>();
            Bind<DocumenConverter>().ToSelf().InSingletonScope();
            Bind<ISearchProvider>().To<DocumentLuceneBaseSearchProvider>().InSingletonScope();
            Bind<MessageBus>().ToSelf().InSingletonScope();
            Bind<ICachePolicy>().To<MemcachedCachePolicy>().InSingletonScope();
            Bind<ISessionProvider>().To<DefaultSessionProvider>().InSingletonScope();
            Bind<IFormsAuthentication>().To<DefaultFormsAuthentication>().InSingletonScope();
            Bind<ContextService>().ToSelf().InSingletonScope();
            Bind<IStorePolicy>().To<MongoPolicy>().InSingletonScope();
        }
        
    }
}