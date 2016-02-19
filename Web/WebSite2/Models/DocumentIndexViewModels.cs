using System.Collections.Generic;
using Services.Contracts;

namespace WebSite2.Models
{
   
    public class DocumentIndexViewModel : BaseMenuViewModel
    {
        /// <summary>
        /// 所有的文档
        /// </summary>
        public DocumentViewModelCollection DocumentModels { get; set; }


        public string PageCode { get; set; }
    }
}