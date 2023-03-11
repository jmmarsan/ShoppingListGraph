using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingListGraph.Models
{
    public class ThingsToBuy
    {
        public string Id { get; set; }
        public List<ShoppingListGraph.Models.ListElement> Elements { get; set; }

        public ThingsToBuy() {
            Elements = new List<ListElement>();
        }
    }
}