using System.Collections.Generic;
using Services.Contracts;

namespace WebSite2.Models
{
    public class HomeViewModel : BaseMenuViewModel
    {
        /// <summary>
        /// 所有的文档
        /// </summary>
        public DocumentViewModelCollection Documents { get; set; }

        /// <summary>
        /// 当前空间
        /// </summary>
        public SpaceViewModel CurrentSpace { get; set; }
   

        /// <summary>
        /// 父级空间
        /// </summary>
        public SpaceViewModelCollection ParentSpaces { get; set; }
      
        /// <summary>
        /// 所有空间
        /// </summary>
        public SpaceViewModelCollection ChildSpaces { get; set; }
     
        /// <summary>
        /// 分页代码
        /// </summary>
        private string _pageCode;
        public string PageCode
        {
            get
            {
                return _pageCode;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _pageCode != value)
                    _pageCode = value;
            }
        }

    }
}