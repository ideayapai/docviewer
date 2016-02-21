using System.Collections.Generic;
using System.Linq;
using Services.Search;

namespace WebSite2.Models
{
    public static class SearchResultViewModelConversion
    {
        public static List<SearchResultViewModel> ToList(this List<DocumentSearchResult> list)
        {
            return list.Select(searchResult => new SearchResultViewModel(searchResult)).ToList();
        }
    }
}