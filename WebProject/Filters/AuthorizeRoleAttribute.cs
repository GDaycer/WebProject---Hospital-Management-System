using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebProject.Filters
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedRoles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _allowedRoles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["UserId"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary {
                        { "controller", "Account" },
                        { "action", "Login" }
                    }
                );
                return;
            }

            var userRoleSession = HttpContext.Current.Session["UserRole"];

            if (userRoleSession == null)
            {
                // Should not happen if UserId is present, but safety first
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } }
                );
                return;
            }

            string userRole = userRoleSession.ToString();

            // If the user's role is NOT in the list of allowed roles
            if (!_allowedRoles.Contains(userRole))
            {
                // Redirect to Access Denied
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary {
                        { "controller", "Account" },
                        { "action", "AccessDenied" }
                    }
                );
            }

            base.OnActionExecuting(filterContext);
        }
    }
}