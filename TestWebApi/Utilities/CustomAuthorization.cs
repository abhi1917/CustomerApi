using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TestWebApi.Utilities
{
    public class CustomAuthorization:AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                var authToken = ConfigurationManager.AppSettings["authToken"].ToString();
                if (actionContext.Request.Headers.Authorization != null) 
                {
                    if(actionContext.Request.Headers.Authorization.Parameter== authToken)
                    {
                        base.OnAuthorization(actionContext);
                    }
                    else
                    {
                        actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    }
                }
                else
                {
                    actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                }
            }
            catch(Exception ex)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                return;
            }
        }
    }
}