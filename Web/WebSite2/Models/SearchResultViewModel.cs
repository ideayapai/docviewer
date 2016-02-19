using System;
using Documents;
using Documents.Enums;
using Services.Search;

namespace WebSite2.Models
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel()
        {
            
        }

        public SearchResultViewModel(DocumentSearchResult searchResult)
        {
            SearchResult = searchResult;
        }

        public DocumentSearchResult SearchResult { get; set; }

        public DocumentType DocumentType
        {
            get
            {
                return (DocumentType)Enum.Parse(typeof(DocumentType), SearchResult.DocumentType);
            }
            
        }

        public string MainUrl
        {
            get
            {
                string url = "/#";

                if (!string.IsNullOrEmpty(SearchResult.DocumentType))
                {
                    switch ((DocumentType)Enum.Parse(typeof(DocumentType), SearchResult.DocumentType))
                    {
                        case DocumentType.Folder:
                            url = "/Home/Space?spaceid=" + SearchResult.Id;
                            break;

                        default:
                            url = "/View/Index?Id=" + SearchResult.Id;
                            break;
                    }
                }
                  

                return url;
            }
        }
    }
}