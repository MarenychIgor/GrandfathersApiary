﻿using Sitecore.Data;
using System.Collections.Generic;

namespace GrandfathersApiary.Feature.Navigation.Models
{
    public class NavigationItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public ID TemplateId { get; set; }
        public ID ItemId { get; set; }
        public bool ShowChildren { get; set; }
        public bool IsActive { get; set; }
        public bool HasActiveItemOnLowerLayer { get; set; }
        public List<NavigationItem> Children { get; set; } = new List<NavigationItem>();
    }
}