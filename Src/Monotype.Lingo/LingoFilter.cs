#nullable enable

using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Monotype.Localization;

public class LingoFilter : IAsyncActionFilter, IAsyncPageFilter
{
	// Constructors
	public LingoFilter(Lingo lingo)
	{
		this.Lingo = lingo;
	}


	// Methods - Controller
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		if(context.Controller is Controller controller)
		{
			var controllerType = controller.GetType();
			var actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;

			var lingoCtrlAttr = controllerType.GetCustomAttribute<LingoPrefixAttribute>(true);

			var prefix = "";

			if(lingoCtrlAttr != null)
			{
				prefix = lingoCtrlAttr.Prefix.TrimToNull();

				if(prefix.IsNullOrWhiteSpace())
				{
					prefix = this.Lingo.GetBasePrefix(controllerType).UnSuffix(".");
				}
			}

			if(actionDescriptor != null)
			{
				var lingoActionAttr = actionDescriptor.MethodInfo.GetCustomAttribute<LingoPrefixAttribute>(true);

				if(lingoActionAttr != null || lingoCtrlAttr != null)
				{
					var actionPrefix = lingoActionAttr?.Prefix.TrimToNull();
						
					if(actionPrefix.IsNullOrWhiteSpace())
					{
						actionPrefix = actionDescriptor.MethodInfo.Name.UnSuffix("Async");

						if(lingoCtrlAttr != null)
						{
							actionPrefix = actionPrefix.Prefix(".");
						}
					}

					prefix = actionPrefix.IsPrefixed(".") ? prefix + actionPrefix : actionPrefix;
				}
			}

			if(!prefix.IsNullOrWhiteSpace())
			{
				context.HttpContext.Items[$"Lingo.Prefix"] = prefix;
			}
		}

		await next();
	}


	// Methods - Page
	public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
	{
		await next();
	}

	public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
	{
		var handlerType = context.HandlerInstance.GetType();

		if(handlerType != null)
		{
			var lingoModelAttr = handlerType.GetCustomAttribute<LingoPrefixAttribute>(true);

			if(lingoModelAttr != null)
			{
				var prefix = lingoModelAttr.Prefix.TrimToNull();

				if(prefix.IsNullOrWhiteSpace())
				{
					prefix = this.Lingo.GetBasePrefix(handlerType).UnSuffix(".");
				}

				context.HttpContext.Items[$"Lingo.Prefix"] = prefix;
			}

			if(context.HandlerMethod != null)
			{
				var lingoHandlerAttr = context.HandlerMethod.MethodInfo.GetCustomAttribute<LingoPrefixAttribute>(true);

				if(lingoHandlerAttr != null)
				{
					var prefix = lingoHandlerAttr.Prefix;

					if(prefix.IsPrefixed("."))
					{
						prefix = (context.HttpContext.Items[$"Lingo.Prefix"] as String) + prefix;
					}

					context.HttpContext.Items[$"Lingo.Prefix"] = prefix;
				}
			}
		}

		return Task.CompletedTask;
	}



	#region Protected Area

	private readonly Lingo Lingo;

	#endregion

}