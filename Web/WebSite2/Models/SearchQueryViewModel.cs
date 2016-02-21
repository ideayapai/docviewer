using System.Collections.Generic;
using System.Web.Mvc;
using Infrasturcture.Utils;

namespace WebSite2.Models
{
    public class SearchQueryViewModel
    {
        public string QueryString { get; set; }
        
        public SearchQueryViewModel()
        {
            
        }

        /// <summary>
        /// 查询关键字
        /// </summary>
        /// <param name="queryString"></param>
        public SearchQueryViewModel(string queryString)
        {
            QueryString = queryString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryString">查询关键字</param>
        /// <param name="documentType">文档类型</param>
        /// <param name="time">时间范围</param>
        /// <param name="count">分页数量</param>
        public SearchQueryViewModel(string queryString, string documentType, string time, string count)
        {
            QueryString = queryString;
            DocumentType = documentType;
            Count = count;
            Time = time;
        }

        private List<SelectListItem> _documentTypes;
        public List<SelectListItem> DocumentTypes
        {
            get
            {
                if (_documentTypes == null || _documentTypes.Count == 0) 
                {
                    _documentTypes = new List<SelectListItem>
                                                            {
                                                                new SelectListItem() {Text = "全部", Value = string.Empty, Selected = true},
                                                                new SelectListItem() {Text = "Word", Value = Documents.Enums.DocumentType.Word.ToString(), Selected = false},
                                                                new SelectListItem() {Text = "Excel", Value = Documents.Enums.DocumentType.Excel.ToString(), Selected = false},
                                                                new SelectListItem() {Text ="PPT", Value = Documents.Enums.DocumentType.PPT.ToString(), Selected = false},
                                                                new SelectListItem() {Text ="PDF", Value = Documents.Enums.DocumentType.PDF.ToString(), Selected = false},
                                                                new SelectListItem() {Text = "CAD", Value = Documents.Enums.DocumentType.TXT.ToString(), Selected = false}
                                                            };
                }
                return _documentTypes;
            }
            set
            {
                if (value != null)
                {
                    _documentTypes = value;
                }
            }
        }

        private string _documentType;
        public string DocumentType
        {
            get { return _documentType; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    foreach (var documentType in DocumentTypes)
                    {
                        documentType.Selected = documentType.Value == value;
                    }
                }
                _documentType = value;
            }
        }

        public string Begin
        {
            get
            {
                return DateTimeUtils.GetBegin(Time);
            }
        }

        public string End
        {
            get
            {    
                return DateTimeUtils.GetEnd(Time);
            }
        }
        
        private List<SelectListItem> _times;
        public List<SelectListItem> Times
        {
            get
            {
                
                if (_times == null)
                {
                    _times = new List<SelectListItem>{
                                                            new SelectListItem() {Text = "任意时间", Value = string.Empty, Selected = true},
                                                            new SelectListItem() {Text = "今天", Value = DateTimeConstants.Today.ToString(), Selected = false},
                                                            new SelectListItem() {Text = "昨天", Value = DateTimeConstants.Yesterday.ToString(), Selected = false},
                                                            new SelectListItem() {Text = "最近7天", Value = DateTimeConstants.LastWeek.ToString(), Selected = false},
                                                            new SelectListItem() {Text = "最近30天", Value = DateTimeConstants.LastMonth.ToString(), Selected = false},
                                                        };
                }
                return _times;
            }

            set
            {
                if (value != null)
                {
                    _times = value;
                }
            }
        }

        private string _time;
        public string Time
        {
            get { return _time; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    foreach (var time in Times)
                    {
                        time.Selected = time.Value == value;
                    }
                }
                _time = value;
            }
        }
        
        private List<SelectListItem> _counts;
        public List<SelectListItem> Counts
        {
            get
            {
                if (_counts == null)
                {
                    _counts = new List<SelectListItem>()
                                                            {
                                                                new SelectListItem() {Text = "5", Value = "5", Selected = true},
                                                                new SelectListItem() {Text = "10", Value = "10", Selected = false},
                                                                new SelectListItem() {Text = "20", Value = "2", Selected = false},
                                                            };
                }
                return _counts;
            }
            set
            {
                if (value != null)
                {
                    _counts = value;
                }
            }
        }

        private string _count;
        public string Count
        {
            get { return _count; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    foreach (var count in Counts)
                    {
                        count.Selected = count.Value == value;
                    }
                }
                _count = value;
            }
        }
       
    }
}