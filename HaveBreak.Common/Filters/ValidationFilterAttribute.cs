using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaveBreak.Shared;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using HaveBreak.Shared.ResultDtos;

namespace HaveBreak.Common.Filters
{
    public class ValidationFilterAttribute : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var factory = context.HttpContext.RequestServices.GetService<IStringLocalizerFactory>();

            var localizer = factory.Create(typeof(CommonResource));

            // execute any code before the action executes
            if (!context.ModelState.IsValid)
            {
                var modelState = context.ModelState;

                var validationErrors = modelState.Keys
                        .SelectMany(key => modelState[key].Errors.Select(x => new ValidationErrorDto(key, x.ErrorMessage)))
                        .ToList();

                context.Result = new BadRequestObjectResult(ErrorResultDto.CreateValidationError(validationErrors, localizer));
                return;
            }

            await next();

            // execute any code after the action executes
        }
    }
}
