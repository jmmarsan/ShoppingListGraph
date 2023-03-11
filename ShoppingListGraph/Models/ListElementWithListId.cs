using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingListGraph.Models
{
    public class ListElementWithListId: ListElement
    {
        public string ListId { get; set; }
    }
}