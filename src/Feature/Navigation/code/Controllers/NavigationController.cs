using GrandfathersApiary.Feature.Navigation.Constants;
using GrandfathersApiary.Feature.Navigation.Models;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Sites;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
            var childrenItems = rootItem.Children.Where(IsAllowedToShowInHeader)
                .OrderByDescending(x => x.TemplateID == Templates.LandingPageTemplate.ID);

            var homePage = GetNavigationItem(rootItem, currentUrl, FieldNames.ShowInHeaderNavigation, getChildren: false);
            var children = childrenItems.Select(x => GetNavigationItem(x, currentUrl, FieldNames.ShowInHeaderNavigation)).ToList();

            result.Add(homePage);
            result.AddRange(children);

            return result;
        }

        private NavigationItem GetAsideNavigationHierarchy(Item rootItem)
        {
            var currentUrl = LinkManager.GetItemUrl(Context.Item);

            var homePage = GetNavigationItem(rootItem, currentUrl, FieldNames.ShowInAsideNavigation);
            SortChildren(homePage);

            return homePage;
        }

        private void SortChildren(NavigationItem homePage)
        {
            var searchPage = homePage.Children.FirstOrDefault(x => x.ItemId == Templates.SearchPage.ID);

            homePage.Children = homePage.Children
                .Where(x => x.ItemId != Templates.SearchPage.ID)
                .OrderByDescending(x => x.TemplateId == Templates.LandingPageTemplate.ID)
                .ToList();

            if (searchPage != null)
            {
                homePage.Children.Add(searchPage);
            }
        }

        private NavigationItem GetNavigationItem(Item item, string currentUrl, string showInFieldName, bool getChildren = true)
        {
            var url = LinkManager.GetItemUrl(item);
            
            var result = new NavigationItem
            {
                Title = item.Fields[FieldNames.Title].ToString(),
                Url = url,
                TemplateId = item.TemplateID,
                ItemId = item.ID,
                ShowChildren = MainUtil.GetBool(item[FieldNames.ShowChildren], defaultValue: false),
                IsActive = url == currentUrl,
                Children = getChildren ? GetChildren(item, currentUrl, showInFieldName) : new List<NavigationItem>()
            };

            return result;
        }

        private List<NavigationItem> GetChildren(Item item, string currentUrl, string showInFieldName)
        {
            var result = Enumerable.Empty<NavigationItem>();

            if (item.Children.Any())
            {
                result = item.Children.Where(x => MainUtil.GetBool(x[showInFieldName], defaultValue: false))
                                      .Select(x => GetNavigationItem(x, currentUrl, showInFieldName));
            }

            return result.ToList();
        }

        private bool IsAllowedToShowInHeader(Item item)
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