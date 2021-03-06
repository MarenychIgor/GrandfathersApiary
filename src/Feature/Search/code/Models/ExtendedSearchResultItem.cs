﻿using Sitecore.ContentSearch.SearchTypes;

namespace GrandfathersApiary.Feature.Search.Models
{
    public class ExtendedSearchResultItem : SearchResultItem
    {
        public string Keywords { get; set; }
        public string NestingLevel { get; set; }
    }
}