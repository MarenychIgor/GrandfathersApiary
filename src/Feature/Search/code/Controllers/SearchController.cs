using GrandfathersApiary.Feature.Search.Models;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Links;

namespace GrandfathersApiary.Feature.Search.Controllers
{
    public class SearchController : Controller
    {
        private Database Db { get; }
        private const string IndexName = "content_nesting_level_index";

        public SearchController()
        {
            Db = Context.Database;
        }

        [HttpPost]
        public ActionResult AjaxSearchResults(string query)
        {
            var searchResult = Enumerable.Empty<SearchResultViewModel>();

            var index = ContentSearchManager.GetIndex(IndexName);

            var isSearchByNestingLevel = int.TryParse(query, out int level);

            using (var searchContext = index.CreateSearchContext())
            {
                var result = isSearchByNestingLevel
                    ? SearchByNestingLevel(searchContext, level)
                    : SearchByTitle(searchContext, query);

                searchResult = result.Hits.Select(x => Map(x.Document)).ToList();
            }

            return new JsonResult { Data = searchResult };
        }

        private SearchResultViewModel Map(ExtendedSearchResultItem searchItem)
        {
            var item = Db.GetItem(searchItem.ItemId);
            var url = LinkManager.GetItemUrl(item);

            var result = new SearchResultViewModel
            {
                Name = searchItem.Name,
                Url = url
            };

            return result;
        }

        private SearchResults<ExtendedSearchResultItem> SearchByNestingLevel(IProviderSearchContext searchContext, int level)
        {
            var result = searchContext.GetQueryable<ExtendedSearchResultItem>()
                .Where(x => x.NestingLevel == level.ToString())
                .GetResults();

            return result;
        }

        private SearchResults<ExtendedSearchResultItem> SearchByTitle(IProviderSearchContext searchContext, string query)
        {
            var result = searchContext.GetQueryable<ExtendedSearchResultItem>()
                .Where(x => x.Keywords.Contains(query))
                .GetResults();

            return result;
        }
    }
}