using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingListGraph.Models
{
    public class ListElement
    {
        public string ListId { get; set; }
        public string Id { get; set; }
        public bool Completed { get; set; }
        public string Title { get; set; }
        public bool HighPriority { get; set; }
        public DateTimeOffset? CreatedDateTime { get; set; }
    }
}