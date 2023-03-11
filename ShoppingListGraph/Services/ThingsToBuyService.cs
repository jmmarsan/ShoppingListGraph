using Microsoft.Graph;
using ShoppingListGraph.Helpers;
using ShoppingListGraph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShoppingListGraph.Services
{
    public class ThingsToBuyService
    {

        public async Task<ThingsToBuy> GetData()
        {
            //Get data
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

            return new ThingsToBuy
            {
                Id = shoppingList.Id,
                Elements = elements.OrderByDescending(t => t.CreatedDateTime).OrderByDescending(t => t.HighPriority).OrderBy(t => t.Completed).ToList()
            };
        }

        public async void SaveChanges(ThingsToBuy edit)
        {
            foreach (ListElement element in edit.Elements)
            {
                TodoTask task = await GraphHelper.PatchTodoTaskAsync(edit.Id, element.Id, element.Completed);
            }
        }
    }
}