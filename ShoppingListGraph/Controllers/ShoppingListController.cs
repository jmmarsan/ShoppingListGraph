using Microsoft.Graph;
using ShoppingListGraph.Helpers;
using ShoppingListGraph.Models;
using ShoppingListGraph.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShoppingListGraph.Controllers
{
    public class ShoppingListController : BaseController
    {
        // GET: ShoppingList
        [Authorize]
        public async Task<ActionResult> Index()
        {
            ThingsToBuyService service = new ThingsToBuyService();
            ThingsToBuy thingsToBuy = await service.GetData();
            return View( thingsToBuy);
        }

        // POST: ShoppingList
        [Authorize, HttpPost]
        public async Task<ActionResult> Index(ThingsToBuy edit)
        {
            ThingsToBuyService service = new ThingsToBuyService();
            //Save changes
            ThingsToBuy thingsToBuy = await service.SaveChanges(edit);
            return View(thingsToBuy);
        }

        //GET: Create ListElement
        [Authorize]
        public ActionResult Create(string id)
        {
            return View( new ListElementWithListId() { Id = null, ListId = id});
        }

        [Authorize, HttpPost]
        public async Task<ActionResult> Create(ListElementWithListId listElement)
        {
            ThingsToBuyService service = new ThingsToBuyService();
            //Save changes
           await service.CreateListElement(listElement);
            return RedirectToAction("Index");
        }
    }
}