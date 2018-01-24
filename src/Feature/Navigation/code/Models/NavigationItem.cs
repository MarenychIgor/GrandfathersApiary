using System.Collections.Generic;

namespace GrandfathersApiary.Feature.Navigation.Models
{
    public class NavigationItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public bool ShowChildren { get; set; }
        public bool IsActive { get; set; }
        public List<NavigationItem> Children { get; set; } = new List<NavigationItem>();
    }
}