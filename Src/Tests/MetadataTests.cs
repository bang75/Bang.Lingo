using System.Globalization;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using Bang.Lingo;
using Bang.Lingo.Extensions;

namespace LingoTests;


public class BaseModel
{
}

public class TestModel1 : BaseModel
{
	public Int32 Id { get; set; }

	public String? Name { get; set; }

	public String? Email { get; set; }

	public String? Phone { get; set; }
}

public class CompositeMetadataDetailsProvider : ICompositeMetadataDetailsProvider
{
	private readonly IEnumerable<IMetadataDetailsProvider> _providers;

	public CompositeMetadataDetailsProvider(IEnumerable<IMetadataDetailsProvider> providers)
	{
		_providers = providers;
	}

	/// <inheritdoc />
	public void CreateBindingMetadata(BindingMetadataProviderContext context)
	{
		if(context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		foreach(var provider in _providers.OfType<IBindingMetadataProvider>())
		{
			provider.CreateBindingMetadata(context);
		}
	}

	/// <inheritdoc />
	public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
	{
		if(context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		foreach(var provider in _providers.OfType<IDisplayMetadataProvider>())
		{
			provider.CreateDisplayMetadata(context);
		}
	}

	/// <inheritdoc />
	public void CreateValidationMetadata(ValidationMetadataProviderContext context)
	{
		if(context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		foreach(var provider in _providers.OfType<IValidationMetadataProvider>())
		{
			provider.CreateValidationMetadata(context);
		}
	}
}


[TestClass]
public class MetadataTests
{
	[TestMethod]
	public void BasicTest()
	{
		var options = new LingoOptions();

		options.BasePrefixes.Add(typeof(BaseModel), "BaseModels");
		options.BasePrefixes.Add(typeof(TestModel1), ".TestModels");

		var lingo = new Lingo(options);

		lingo.AddXml("i18n/Translations-1.sv.xml");
		lingo.AddXml("i18n/Translations-1.en.xml");
		lingo.AddXml("i18n/Translations-2.sv.xml");
		lingo.AddXml("i18n/Translations-2.en.xml");

		lingo.Load();

		var detailsProviders = new IMetadataDetailsProvider[]
		{
			new DisplayMetadataProvider(lingo)
		};

		var provider = new DefaultModelMetadataProvider(new CompositeMetadataDetailsProvider(detailsProviders));

		var metadata = provider.GetMetadataForType(typeof(TestModel1));

		var idMeta = metadata.Properties.FirstOrDefault(m => m.PropertyName == nameof(TestModel1.Id));
		var idName = metadata.Properties.FirstOrDefault(m => m.PropertyName == nameof(TestModel1.Name));

		Thread.CurrentThread.CurrentUICulture = new CultureInfo("sv");

		Assert.AreEqual("ID-nummer", idMeta?.DisplayName);
		Assert.AreEqual("Identifierar objektet.", idMeta?.Description);

		Assert.AreEqual("Namn", idName?.DisplayName);
		Assert.AreEqual("Objektets namn.", idName?.Description);
	}
}