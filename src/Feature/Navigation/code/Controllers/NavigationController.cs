using GrandfathersApiary.Feature.Navigation.Constants;
using GrandfathersApiary.Feature.Navigation.Models;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Links;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Sites;

namespace GrandfathersApiary.Feature.Navigation.Controllers
{
    public class NavigationController : Controller
    {
        private readonly SiteContext site = Context.Site;
        private readonly Database database = Context.Site.Database;

        public ActionResult HeaderNavigationMenu()
        {
            var rootItem = database.GetItem(site.StartPath);
            var model = GetHeaderNavigationModel(rootItem);

            return View("HeaderNavigationPartial", model);
        }

        public ActionResult AsideNavigationMenu()
        {
            var rootItem = database.GetItem(site.StartPath);
            var model = GetAsideNavigationModel(rootItem);

            return View("AsideNavigationPartial", model);
        }

        private NavigationViewModel GetHeaderNavigationModel(Item rootItem)
        {
            var result = new NavigationViewModel
            {
                Items = GetHeaderNavigationHierarchy(rootItem)
            };

            return result;
        }

        private NavigationViewModel GetAsideNavigationModel(Item rootItem)
        {
            var result = new NavigationViewModel();

            var navigationModel = GetAsideNavigationHierarchy(rootItem);
            MarkIfHasActiveItemOnLowerLevel(navigationModel);

            result.Items.Add(navigationModel);

            return result;
        }

        private List<NavigationItem> GetHeaderNavigationHierarchy(Item rootItem)
        {
            var result = new List<NavigationItem>();

            var currentUrl = LinkManager.GetItemUrl(Context.Item);
            var childrenItems = rootItem.Children.Where(IsAllowedToShow)
                .OrderByDescending(x => x.TemplateName == TemplateNames.LandingPage);

            var homePage = GetNavigationItem(rootItem, currentUrl, getChildren: false);
            var children = childrenItems.Select(x => GetNavigationItem(x, currentUrl)).ToList();

            result.Add(homePage);
            result.AddRange(children);

            return result;
        }

        private NavigationItem GetAsideNavigationHierarchy(Item rootItem)
        {
            var currentUrl = LinkManager.GetItemUrl(Context.Item);

            var homePage = GetNavigationItem(rootItem, currentUrl);
            homePage.Children = homePage.Children.OrderByDescending(x => x.TemplateName == TemplateNames.LandingPage).ToList();

            return homePage;
        }

        private NavigationItem GetNavigationItem(Item item, string currentUrl, bool getChildren = true)
        {
            var url = LinkManager.GetItemUrl(item);

            var result = new NavigationItem
            {
                Title = item.Fields[FieldNames.Title].ToString(),
                Url = url,
                TemplateName = item.TemplateName,
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

        public void MarkIfHasActiveItemOnLowerLevel(NavigationItem home)
        {
            home.HasActiveItemOnLowerLayer = HasActiveItemOnLowerLevel(home);
        }

        private bool HasActiveItemOnLowerLevel(NavigationItem item)
        {
            foreach (var child in item.Children)
            {
                if (child.IsActive || HasActiveItemOnLowerLevel(child))
                {
                    item.HasActiveItemOnLowerLayer = true;
                    return true;
                }
            }

            return false;
        } 
    }
}