using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Project.Areas.Admin.Attributes
{
    public class CheckLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            //Lay gia tri cua bien Session
            string _email = context.HttpContext.Session.GetString("admin_email");
            //neu session ko ton tai thi di chuyen den login
            if (string.IsNullOrEmpty(_email))
            {
                context.HttpContext.Response.Redirect("/Admin/Account/Login");
            }
            base.OnActionExecuting(context);
        }
    }
}
