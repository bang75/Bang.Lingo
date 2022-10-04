#nullable enable

using System.Reflection;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

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
		if(context.Key.MetadataKind == ModelMetadataKind.Type)
		{
			if(context.Key.ContainerType?.IsAssignableTo(typeof(ControllerBase)) != true)
			{
				var typePrefix = context.TypeAttributes?.AsEnumerable().OfType<LingoPrefixAttribute>().FirstOrDefault()?.Prefix;

				if(typePrefix.IsNullOrWhiteSpace())
				{
					typePrefix = this.Lingo.GetBasePrefix(context.Key.ModelType).Suffix(".") + context.Key.ModelType.Name;
				}

				if(context.Key.ModelType.IsEnum)
				{
					var groupedDisplayNamesAndValues = new List<KeyValuePair<EnumGroupAndName, String>>();
					var namesAndValues = new Dictionary<String, String>();

					var enumFields = Enum.GetNames(context.Key.ModelType)
								   .Select(name => context.Key.ModelType.GetField(name)!)
								   .OrderBy(field => field.GetCustomAttribute<DisplayAttribute>(inherit: false)?.GetOrder() ?? 1000);

					foreach(var field in enumFields)
					{
						var groupName = GetDisplayGroup(field);
						var value = ((Enum)field.GetValue(obj: null)!).ToString("d");

						var enumDisplayName = GetDisplayName(field);
							
						var enumGroupAndName = new EnumGroupAndName(groupName, () => this.GetMetaString("", $"{typePrefix}.Values.{field.Name}", enumDisplayName, field.Name));

						groupedDisplayNamesAndValues.Add(new KeyValuePair<EnumGroupAndName, String>(enumGroupAndName, value));
						namesAndValues.Add(field.Name, value);
					}

					context.DisplayMetadata.EnumGroupedDisplayNamesAndValues = groupedDisplayNamesAndValues;
					context.DisplayMetadata.EnumNamesAndValues = namesAndValues;
				}


				var displayName = context.DisplayMetadata.DisplayName;
				context.DisplayMetadata.DisplayName = () => this.GetMetaString("DisplayName", typePrefix, displayName?.Invoke() ?? "", context.Key.ModelType.Name);

				var description = context.DisplayMetadata.Description;
				context.DisplayMetadata.Description = () => this.GetMetaString("Description", typePrefix, description?.Invoke() ?? "", context.Key.ModelType.Name);

				var placeholder = context.DisplayMetadata.Placeholder;
				context.DisplayMetadata.Placeholder = () => this.GetMetaString("Placeholder", typePrefix, placeholder?.Invoke() ?? "", context.Key.ModelType.Name);
			}
		}
		else if(context.Key.MetadataKind == ModelMetadataKind.Property)
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

			var name = context.Key.Name.TrimToNull(context.Key.PropertyInfo?.Name ?? "")!;

			var displayName = context.DisplayMetadata.DisplayName;
			context.DisplayMetadata.DisplayName = () => this.GetMetaString("DisplayName", prefix, displayName?.Invoke() ?? "", name);
				
			var description = context.DisplayMetadata.Description;
			context.DisplayMetadata.Description = () => this.GetMetaString("Description", prefix, description?.Invoke() ?? "", name);

			var placeholder = context.DisplayMetadata.Placeholder;
			context.DisplayMetadata.Placeholder = () => this.GetMetaString("Placeholder", prefix, placeholder?.Invoke() ?? "", name);
		}
	}



	#region Protected Area
		
	protected Lingo Lingo;


	// Methods

	private String GetMetaString(String type, String? prefix, String? val, String name)
	{
		var i18n = this.Lingo.GetTranslator();

		var metaString = val;

		if(metaString.IsNullOrWhiteSpace())
		{
			metaString = i18n.Translate(prefix + type.Prefix("."), nullIfNotExists: true);

			if(metaString == null && (type == "DisplayName" || type.IsNullOrWhiteSpace()))
			{
				metaString = name.ToReadable();
			}
		}
		else if(metaString.IsPrefixed("#"))
		{
			metaString = i18n[metaString!];
		}

		return metaString ?? "";
	}

	private static String? GetDisplayName(FieldInfo field) =>  field.GetCustomAttribute<DisplayAttribute>(inherit: false)?.GetName();

	private static String GetDisplayGroup(FieldInfo field) => field.GetCustomAttribute<DisplayAttribute>(inherit: false)?.GetGroupName() ?? String.Empty;

	#endregion

}