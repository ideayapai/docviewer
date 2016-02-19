using System.Collections.Generic;

namespace WebSite2.Models
{
    public class SearchViewModel : BaseMenuViewModel
    {
        public SearchViewModel()
        {
            
        }

        public SearchViewModel(List<SearchResultViewModel> searchResultViewModel, 
                              SearchQueryViewModel searchQueryViewModel)
        {
            _searchResultViewModel = searchResultViewModel;
            _searchQueryViewModel = searchQueryViewModel;
        }
        /// <summary>
        /// 搜索条件视图
        /// </summary>
        private SearchQueryViewModel _searchQueryViewModel;
        public SearchQueryViewModel SearchQueryViewModel
        {
            get
            {
                if (_searchQueryViewModel == null)
                {
                    _searchQueryViewModel = new SearchQueryViewModel();
                }
                return _searchQueryViewModel;
            }

            set
            {
                if (value != null)
                {
                    _searchQueryViewModel = value;
                }
            }
        }


        private List<SearchResultViewModel> _searchResultViewModel;
        public List<SearchResultViewModel> SearchResultViewModel
        {
            get
            {
                if (_searchResultViewModel == null)
                {
                    _searchResultViewModel = new List<SearchResultViewModel>();
                }
                return _searchResultViewModel;
            }

            set
            {
                if (value != null)
                {
                    _searchResultViewModel = value;
                }
            }
        }

        public double ElapseTime { get; set; }
       
    }
}