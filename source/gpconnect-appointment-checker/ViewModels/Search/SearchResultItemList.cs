using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.ViewModels.Search
{
    public class SearchResultItemList : List<SearchResultItem>
    {
        public TimeSpan SearchTook { get; set; }
    }
}
