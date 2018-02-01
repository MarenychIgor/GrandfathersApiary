using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;
using System.Linq;

namespace GrandfathersApiary.Feature.Search.Models
{
    public class PageNestingLevel : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        public virtual object ComputeFieldValue(IIndexable indexable)
        {
            var nestingLevel = default(int);

            var db = Sitecore.Configuration.Factory.GetDatabase("master");
            var homePageItem = db.GetItem(Templates.HomePageItem.ID);
            Item item = indexable as SitecoreIndexableItem;

            if (item == null || !item.Axes.IsDescendantOf(homePageItem))
            {
                return null;
            }

            return item.ID == homePageItem.ID ? nestingLevel : GetNestingLevel(homePageItem, item, nestingLevel);
        }

        private int? GetNestingLevel(Item parent, Item searchedItem, int nestingLevel)
        {
            if (IsItemOnNextLayer(parent, searchedItem))
            {
                return ++nestingLevel;
            }
            
            foreach (Item child in parent.Children)
            {
                var level = GetNestingLevel(child, searchedItem, ++nestingLevel);

                if (level != null)
                {
                    return level;
                }
            }

            return null;
        }

        private bool IsItemOnNextLayer(Item parent, Item searchedItem)
        {
            return parent.Children.Any(x => x.ID == searchedItem.ID);
        }
    }
}