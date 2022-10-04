#nullable enable

using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Bang.Lingo;

public class LingoFilter : IAsyncActionFilter, IAsyncPageFilter
{
	// Constructors
	public LingoFilter()
	{
	}


	// Methods
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		if(context.Controller is Controller controller)
		{
			var actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
			var lingoAttr = actionDescriptor?.MethodInfo.GetCustomAttribute<LingoPrefixAttribute>(true)
				?? controller.GetType().GetCustomAttribute<LingoPrefixAttribute>(true);

			if(lingoAttr != null)
			{
				context.HttpContext.Items["Lingo.Prefix"] = lingoAttr?.Prefix;
			}
		}

		await next();
	}

	public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
	{
		await next();
	}

	public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
	{
		var lingoAttr = context.HandlerMethod?.MethodInfo?.GetCustomAttribute<LingoPrefixAttribute>(true);

		var prefix = lingoAttr?.Prefix;

		if(prefix == null || prefix.IsPrefixed("."))
		{
			var handlerType = context.HandlerInstance.GetType();

			lingoAttr = handlerType.GetCustomAttribute<LingoPrefixAttribute>(true);

			prefix = (lingoAttr?.Prefix ?? handlerType.Name.UnSuffix("Model")) + prefix;
		}

		context.HttpContext.Items["Lingo.Prefix"] = prefix;

		return Task.CompletedTask;
	}



	#region Protected Area
	#endregion

}