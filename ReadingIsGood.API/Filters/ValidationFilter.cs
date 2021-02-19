using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ReadingIsGood.API.Models;

namespace ReadingIsGood.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState.Where(t => t.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage)).ToArray();


                var errorResponse = new ErrorResponse();
                foreach (var error in errorsInModelState)
                foreach (var subError in error.Value)
                {
                    var errorModel = new ErrorModel
                    {
                        Message = subError,
                        FieldName = error.Key
                    };

                    errorResponse.Errors.Add(errorModel);
                }

                context.Result = new BadRequestObjectResult(errorResponse);
                return;
            }

            await next();
        }
    }
}