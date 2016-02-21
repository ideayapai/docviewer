using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Logging;
using Documents;
using Documents.Enums;
using Infrasturcture.Store;
using Infrasturcture.Web;
using Services.Documents;
using Services.Spaces;
using WebAPI2.Models;

namespace WebAPI2.Controllers
{
    /// <summary>
    /// 预览界面
    /// </summary>
    public class ViewController:Controller
    {
        private readonly DocumentService _documentService;
        private readonly IStorePolicy _storePolicy;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

       
        public ViewController(DocumentService documentService, IStorePolicy storePolicy)
        {
            _documentService = documentService;
            _storePolicy = storePolicy;
        }

        public ActionResult Index(string Id)
        {
            _logger.InfoFormat("进入浏览界面 {0}", Id);

            Guid result;
            if (string.IsNullOrWhiteSpace(Id) || !Guid.TryParse(Id, out result))
            {
                return View("NotFound", new DisplayViewModel());
            }

            var document = _documentService.GetDocument(Id);
            if (document == null || document.IsDelete || !_storePolicy.Exist(document.StorePath))
            {
                return View("NotFound", new DisplayViewModel());
            }

         

            switch (document.DocumentCategory)
            {
                case DocumentCategory.Office:
                case DocumentCategory.Text:
                    document = _documentService.Convert(document);
                    return View("Html", new DisplayViewModel { Document = document});

                case DocumentCategory.CAD:
                    document = _documentService.Convert(document);
                    return View("Svg", new DisplayViewModel { Document = document});

                default:
                    return View("Unknow", new DisplayViewModel { Document = document});
            }

        }
    }
}