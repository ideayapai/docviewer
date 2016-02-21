﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Search;
using Services.Spaces;

namespace Services.Search.UnitOfWork
{
    public class RemoveSpaceIndexerxUnitOfWork: IUnitOfWork
    {
        private readonly SpaceService _spaceService;

        private readonly ISearchProvider _searchProvider;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public RemoveSpaceIndexerxUnitOfWork(SpaceService spaceService, ISearchProvider searchProvider)
        {
            _spaceService = spaceService;
            _searchProvider = searchProvider;
        }

        
        public void DoWork()
        {
            _logger.Info("删除回收站中的索引...");

            var spaces = _spaceService.GetAllTrashSpaces();
            if (spaces != null && spaces.Count > 0)
            {
                _searchProvider.DeleteList(spaces);
            }

        }
    }
}
