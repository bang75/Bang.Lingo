#nullable enable

using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using Bang.Lingo.Extensions;
using Bang.Extensions;

namespace Bang.Lingo;

public class DisplayMetadataProvider : IDisplayMetadataProvider
{
	// Constructor
	public DisplayMetadataProvider(Lingo lingo)
	{
		this.Lingo = lingo;
	}


	// Methods
	public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
	{
		//if(context.Key.MetadataKind == ModelMetadataKind.Type)
		//{
		//	if(context.Key.ModelType.IsEnum /*&& !context.Key.ModelType.IsEnumFlags*/)
		//	{
		//		// Ej möjligt eftersom det måste vara dynamiskt då språket är olika...
		//		var enumGroupedDisplayNamesAndValues = new List<KeyValuePair<EnumGroupAndName, String>>();

		//		if(context.DisplayMetadata.EnumGroupedDisplayNamesAndValues != null)
		//		{
		//			foreach(var currentGroup in context.DisplayMetadata.EnumGroupedDisplayNamesAndValues)
		//			{
		//				var displayName = currentGroup.Key.Name;
		//				var name = Enum.GetName(context.Key.ModelType, Int32.Parse(currentGroup.Value));

		//				var transPrefixAttr = context.Attributes.AsEnumerable().OfType<LingoPrefixAttribute>().FirstOrDefault();

		//				if(transPrefixAttr != null)
		//				{
		//					if(displayName.IsPrefixed("#."))
		//					{
		//						displayName = (transPrefixAttr?.Prefix.Suffix(".") + displayName.UnPrefix("#.")).Prefix("#");
		//					}
		//					else if(displayName == name)
		//					{
		//						displayName = (transPrefixAttr?.Prefix.Suffix(".") + name).Prefix("#");
		//					}
		//				}

		//				var group = new EnumGroupAndName(currentGroup.Key.Group, displayName ?? currentGroup.Key.Name);

		//				enumGroupedDisplayNamesAndValues.Add(new KeyValuePair<EnumGroupAndName, String>(group, currentGroup.Value));
		//			}
		//		}

		//		context.DisplayMetadata.EnumGroupedDisplayNamesAndValues = enumGroupedDisplayNamesAndValues;
		//	}
		//}

		//if(context.Key.ContainerType?.IsAssignableTo(typeof(ControllerBase)) != true)


		if(context.Key.MetadataKind == ModelMetadataKind.Property)
		{
			String? prefix = null;

			var containerPrefix = context.Key.ContainerType?.GetCustomAttribute<LingoPrefixAttribute>()?.Prefix;
			var propertyPrefix = context.PropertyAttributes?.AsEnumerable().OfType<LingoPrefixAttribute>().FirstOrDefault()?.Prefix;

			if(!propertyPrefix.IsNullOrWhiteSpace() && !propertyPrefix.IsPrefixed("."))
			{
				prefix = propertyPrefix;
			}
			else
			{
				if(containerPrefix.IsNullOrWhiteSpace() && context.Key.ContainerType != null)
				{
					containerPrefix = this.Lingo.GetBasePrefix(context.Key.ContainerType).Suffix(".") + context.Key.ContainerType?.Name;				
				}

				if(propertyPrefix.IsNullOrWhiteSpace())
				{
					propertyPrefix = $".Fields.{context.Key.Name}";
				}

				prefix = $"{containerPrefix}{propertyPrefix}";
			}


			Func<String, String, String> getMetaString = (String val, String type) =>
			{
				var i18n = Lingo.GetTranslator(Thread.CurrentThread.CurrentUICulture.Name);

				var metaString = val;

				if(metaString.IsNullOrWhiteSpace())
				{
					if(!context.Key.Name.IsNullOrWhiteSpace())
					{
						metaString = i18n.Translate(prefix + type.Prefix(".")!, nullIfNotExists: true);

						if(metaString == null && type == "DisplayName")
						{
							metaString = context.Key.Name.ToReadable();
						}
					}
				}
				else if(metaString.StartsWith('#') == true)
				{
					metaString = i18n[metaString];
				}

				return metaString ?? "";
			};

			var displayName = context.DisplayMetadata.DisplayName;
			context.DisplayMetadata.DisplayName = () => getMetaString(displayName?.Invoke() ?? "", "DisplayName");
				
			var test = context.DisplayMetadata.EnumNamesAndValues;

			var description = context.DisplayMetadata.Description;
			context.DisplayMetadata.Description = () => getMetaString(description?.Invoke() ?? "", "Description");

			var placeholder = context.DisplayMetadata.Placeholder;
			context.DisplayMetadata.Placeholder = () => getMetaString(placeholder?.Invoke() ?? "", "Placeholder");
		}
	}



	#region Protected Area
		
	protected Lingo Lingo;
		
	#endregion

}