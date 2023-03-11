using Azure;
using Microsoft.Graph;
using ShoppingListGraph.Helpers;
using ShoppingListGraph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                Elements = elements
            };
        }

        public async Task<ThingsToBuy> SaveChanges(ThingsToBuy edit)
        {
            foreach (ListElement element in edit.Elements)
            {
                await GraphHelper.PatchTodoTaskAsync(edit.Id, element);
            }
            return await GetData();
        }

        public async Task<HttpStatusCode> CreateListElement(ListElementWithListId element)
        {
            await GraphHelper.PostTodoTaskAsync(element);
            return HttpStatusCode.OK;
        }
    }
}