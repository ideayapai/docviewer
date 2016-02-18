using System.Collections.Generic;
using Ninject;
using Services.Ioc;

namespace Services.Search.UnitOfWork
{
    public class UnitWorkContainer: List<IUnitOfWork>
    {
        public UnitWorkContainer()
        {
            this.Add(ServiceActivator.Get<AddSpaceIndexerxUnitOfWork>());
            this.Add(ServiceActivator.Get<RemoveSpaceIndexerxUnitOfWork>());
            this.Add(ServiceActivator.Get<AddDocumentIndexerxUnitOfWork>());
            this.Add(ServiceActivator.Get<RemoveSpaceIndexerxUnitOfWork>());
        }
    }
}
