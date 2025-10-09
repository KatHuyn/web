using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI_simple.CustomActionFilter
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                // Thay đổi để trả về lỗi chi tiết (400 Bad Request kèm ModelState)
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}