using System.Collections.Generic;
using GrandfathersApiary.Feature.Navigation.Models;
using Sitecore;
using System.Linq;
using System.Web.Mvc;
using GrandfathersApiary.Feature.Navigation.Constants;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace GrandfathersApiary.Feature.Navigation.Controllers
{
    public class NavigationController : Controller
    {
        public ActionResult HeaderNavigationMenu()
        {
            var site = Context.Site;
            var database = site.Database;

            var rootItem = database.GetItem(site.StartPath);
           
            var model = GetHeaderNavigationModel(rootItem);

            return View("HeaderNavigationPartial", model);
        }

        private NavigationViewModel GetHeaderNavigationModel(Item rootItem)
        {
            var result = new NavigationViewModel();

            var currentUrl = LinkManager.GetItemUrl(Context.Item);
            var childrenItems = rootItem.Children.Where(IsAllowedToShow)
                                            .OrderByDescending(x => x.TemplateName == TemplateNames.LandingPage);

            var homePage = GetNavigationItem(rootItem, currentUrl, getChildren: false);
            var children = childrenItems.Select(x => GetNavigationItem(x, currentUrl)).ToList();

            result.Items.Add(homePage);
            result.Items.AddRange(children);

            return result;
        }

        private NavigationItem GetNavigationItem(Item item, string currentUrl, bool getChildren = true)
        {
            var url = LinkManager.GetItemUrl(item);

            var result = new NavigationItem
            {
                Title = item.Fields[FieldNames.Title].ToString(),
                Url = url,
                ShowChildren = MainUtil.GetBool(item[FieldNames.ShowChildren], false),
                IsActive = url == currentUrl,
                Children = getChildren ? GetChildren(item, currentUrl) : new List<NavigationItem>()
            };

            return result;
        }

        private List<NavigationItem> GetChildren(Item item, string currentUrl)
        {
            var result = Enumerable.Empty<NavigationItem>();
            var children = item.Children.Where(IsAllowedToShow).ToList();

            if (children.Any())
            {
                result = children.Select(x => GetNavigationItem(x, currentUrl));
            }

            return result.ToList();
        }

        private bool IsAllowedToShow(Item item)
        {
            var result = item.Fields[FieldNames.Title].HasValue &&
                         MainUtil.GetBool(item[FieldNames.ShowInHeaderNavigation], defaultValue: false);

            return result;
        }
    }
}