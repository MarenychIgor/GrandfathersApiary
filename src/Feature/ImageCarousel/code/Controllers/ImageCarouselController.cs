using GrandfathersApiary.Feature.ImageCarousel.Models;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Data.Fields;

namespace GrandfathersApiary.Feature.ImageCarousel.Controllers
{
    public class ImageCarouselController : Controller
    {
        private readonly string ImageCarouselSlideKey = "Slide";

        public ActionResult MainImageCarousel()
        {
            var model = new ImageCarouselViewModel();

            var site = Context.Site;
            var database = site.Database;
            var item = database.GetItem(Templates.MainImageCarouselRoot.ID);

            model.Slides = item.Children.Select(GetImageItem).ToList();
            model.Slides.First().IsActive = true;

            return View("ImageCarouselPartial", model);
        }

        private ImageCarouselItem GetImageItem(Item item)
        {
            ImageField imageField = item.Fields[ImageCarouselSlideKey];
            var image = new MediaItem(imageField.MediaItem);
            var imageUrl = StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(image));
            
            var result = new ImageCarouselItem
            {
                Path = imageUrl,
                Alt = imageField.Alt
            };

            return result;
        }
    }
}