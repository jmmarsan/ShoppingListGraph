using ShoppingListGraph.TokenStorage;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using ShoppingListGraph.Models;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System;
using Azure.Core;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Xml.Linq;
using System.Net;

namespace ShoppingListGraph.Helpers
{
    public static class GraphHelper
    {
        // Load configuration settings from PrivateSettings.config
        private static string appId = ConfigurationManager.AppSettings["ida:AppId"];
        private static string appSecret = ConfigurationManager.AppSettings["ida:AppSecret"];
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        private static List<string> graphScopes =
            new List<string>(ConfigurationManager.AppSettings["ida:AppScopes"].Split(' '));
        private static string shoppingListDisplayName = ConfigurationManager.AppSettings["literal:ShoppingListDisplayName"];

        #region "Shopping list"

        public static async Task<TodoTaskList> GetTodoTaskListAsync()
        {
            var graphClient = GetAuthenticatedClient();

            var lists = await graphClient.Me.Todo.Lists.Request().GetAsync();
            var shoppingList = lists.Where(l => l.DisplayName == shoppingListDisplayName).FirstOrDefault();
            if (shoppingList == null) {
                shoppingList = await graphClient.Me.Todo.Lists.Request().AddAsync(new TodoTaskList()
                {
                    ODataType = null,
                    DisplayName = shoppingListDisplayName
                });
            }
            return shoppingList;
        }

        public static async Task<TodoTask> PatchTodoTaskAsync(string id, ListElement element)
        {
            var graphClient = GetAuthenticatedClient();

            return await graphClient.Me.Todo.Lists[id].Tasks[element.Id].Request().UpdateAsync(new TodoTask
            {
                ODataType = null,
                Status = element.Completed? Microsoft.Graph.TaskStatus.Completed : Microsoft.Graph.TaskStatus.NotStarted,
                Importance = element.HighPriority? Importance.High : Importance.Normal,
                Title = element.Title
            });
        }


        public static async Task<TodoTask> PostTodoTaskAsync(ListElementWithListId element)
        {
            var graphClient = GetAuthenticatedClient();

            return await graphClient.Me.Todo.Lists[element.ListId].Tasks.Request().AddAsync(new TodoTask
            {
                ODataType = null,
                Status = element.Completed ? Microsoft.Graph.TaskStatus.Completed : Microsoft.Graph.TaskStatus.NotStarted,
                Importance = element.HighPriority ? Importance.High : Importance.Normal,
                Title = element.Title
            });
        }

        public static async Task<IEnumerable<TodoTask>> GetTodoTasksAsync(TodoTaskList taskList) 
        {
            var graphClient = GetAuthenticatedClient();
            var todoTaskList = await graphClient.Me.Todo.Lists[taskList.Id].Tasks.Request().GetAsync();            
            return todoTaskList.CurrentPage.ToList();
        }

        public static async Task<HttpStatusCode> DeleteTodoTaskListAsync(string id)
        {
            var graphClient = GetAuthenticatedClient();
            await graphClient.Me.Todo.Lists[id].Request().DeleteAsync();
            return HttpStatusCode.OK;
        }

        #endregion


        #region "Authentication"

        private static GraphServiceClient GetAuthenticatedClient()
        {
            return new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        var idClient = ConfidentialClientApplicationBuilder.Create(appId)
                            .WithRedirectUri(redirectUri)
                            .WithClientSecret(appSecret)
                            .Build();

                        var tokenStore = new SessionTokenStore(idClient.UserTokenCache,
                                HttpContext.Current, ClaimsPrincipal.Current);

                        var userUniqueId = tokenStore.GetUsersUniqueId(ClaimsPrincipal.Current);
                        var account = await idClient.GetAccountAsync(userUniqueId);

                        // By calling this here, the token can be refreshed
                        // if it's expired right before the Graph call is made
                        var result = await idClient.AcquireTokenSilent(graphScopes, account)
                            .ExecuteAsync();

                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    }));
        }


        public static async Task<CachedUser> GetUserDetailsAsync(string accessToken)
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", accessToken);
                    }));

            var user = await graphClient.Me.Request()
                .Select(u => new {
                    u.DisplayName,
                    u.Mail,
                    u.UserPrincipalName
                })
                .GetAsync();

            return new CachedUser
            {
                Avatar = string.Empty,
                DisplayName = user.DisplayName,
                Email = string.IsNullOrEmpty(user.Mail) ?
                    user.UserPrincipalName : user.Mail
            };
        }

        #endregion
    }


}