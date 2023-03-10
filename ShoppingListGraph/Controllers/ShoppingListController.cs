using Microsoft.Graph;
using ShoppingListGraph.Helpers;
using ShoppingListGraph.Models;
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
            var shoppingList = await GraphHelper.GetTodoTaskListAsync();
            var todoTaskList = await GraphHelper.GetTodoTasksAsync(shoppingList);
            List<ListElement> elements = new List<ListElement>();
            foreach (TodoTask t in todoTaskList)
            {
                elements.Add(new ListElement()
                {
                    Id = t.Id,
                    Completed = (t.Status == Microsoft.Graph.TaskStatus.Completed),
                    Title = t.Title,
                    HighPriority = (t.Importance == Importance.High),
                    CreatedDateTime = t.CreatedDateTime
                });
            }

            return View(new ThingsToBuy { 
                Elements = elements.OrderByDescending(t => t.CreatedDateTime).OrderByDescending(t => t.HighPriority).OrderBy(t => t.Completed).ToList()
            });
        }

        // POST: ShoppingList
        [Authorize, HttpPost]
        public async Task<ActionResult> Index(ThingsToBuy aux)
        {
            var shoppingList = await GraphHelper.GetTodoTaskListAsync();
            var todoTaskList = await GraphHelper.GetTodoTasksAsync(shoppingList);
            List<ListElement> elements = new List<ListElement>();
            foreach (TodoTask t in todoTaskList)
            {
                elements.Add(new ListElement()
                {
                    Id = t.Id,
                    Completed = (t.Status == Microsoft.Graph.TaskStatus.Completed),
                    Title = t.Title,
                    HighPriority = (t.Importance == Importance.High),
                    CreatedDateTime = t.CreatedDateTime
                });
            }

            return View(new ThingsToBuy
            {
                Elements = elements.OrderByDescending(t => t.CreatedDateTime).OrderByDescending(t => t.HighPriority).OrderBy(t => t.Completed).ToList()
            });
        }
    }
}