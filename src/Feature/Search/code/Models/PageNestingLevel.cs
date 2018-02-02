using System.Collections.Generic;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;
using System.Linq;

namespace GrandfathersApiary.Feature.Search.Models
{
    public class PageNestingLevel : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            var db = Sitecore.Configuration.Factory.GetDatabase("web");
            var homePageItem = db.GetItem(Templates.HomePageItem.ID);

            Item item = indexable as SitecoreIndexableItem;

            if (item == null)
            {
                return null;
            }

            var homePageTree = GetHierarchicalModel(homePageItem);
            var foundedItem = homePageTree.FirstOrDefault(x => x.Id == item.ID);

            return foundedItem?.Level;
        }

        public List<ItemHierarchicalModel> GetHierarchicalModel(Item parent)
        {
            var result = new List<ItemHierarchicalModel>();
            var nestingLevel = default(int);

            var root = new ItemHierarchicalModel
            {
                Id = parent.ID,
                Level = nestingLevel
            };

            result.Add(root);
            result.AddRange(GetChildren(parent, nestingLevel));

            return result;
        }

        private List<ItemHierarchicalModel> GetChildren(Item parent, int nestingLevel)
        {
            var result = new List<ItemHierarchicalModel>();

            if (parent.HasChildren)
            {
                nestingLevel++;
                var children = parent.Children
                    .Select(x => Map(x, nestingLevel))
                    .ToList();

                result.AddRange(children);

                foreach (Item child in parent.Children)
                {
                    result.AddRange(GetChildren(child, nestingLevel));
                }
            }

            return result;
        }

        private ItemHierarchicalModel Map(Item item, int nestingLevel)
        {
            return new ItemHierarchicalModel
            {
                Id = item.ID,
                Level = nestingLevel
            };
        }
    }
}