using System.Globalization;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using Monotype.Localization;
using Monotype.Localization.Extensions;

using LingoTests.Models;

namespace LingoTests.UnitTests;

[TestClass]
public partial class MetadataTests
{
	public Lingo CreateLingo(LingoOptions? options = null)
	{
		options = options ?? new LingoOptions();

		options.AddTranslationXml("i18n/Translations-1.sv.xml");
		options.AddTranslationXml("i18n/Translations-1.en.xml");
		options.AddTranslationXml("i18n/Translations-2.sv.xml");
		options.AddTranslationXml("i18n/Translations-2.en.xml");

		var lingo = new Lingo(options);

		lingo.Load();

		return lingo;
	}

	public ModelMetadata GetMetadataForType<T>(Lingo lingo, MissingTranslationMode missingTranslationMode = MissingTranslationMode.AsReadable)
	{
		var detailsProviders = new IMetadataDetailsProvider[]
		{
			new DisplayMetadataProvider(lingo, missingTranslationMode)
		};

		var provider = new DefaultModelMetadataProvider(new CompositeMetadataDetailsProvider(detailsProviders));

		return provider.GetMetadataForType(typeof(T));
	}



	[TestMethod]
    public void Model1SvTest()
    {
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("sv");
		
		var options = new LingoOptions();

        options.AddBasePrefix(typeof(BaseModel), (type) => "BaseModels.");
        options.AddBasePrefix(typeof(Test1Model), "BaseModels.TestModels");

        var lingo = this.CreateLingo(options);


		var metadata = this.GetMetadataForType<BaseModel>(lingo);

		Assert.AreEqual("Basmodel", metadata.DisplayName);


		metadata = this.GetMetadataForType<Test1Model>(lingo);

		Assert.AreEqual("Testmodel 1", metadata.DisplayName);

		var idMeta = metadata.Properties.FirstOrDefault(m => m.PropertyName == nameof(Test1Model.Id));
        var idName = metadata.Properties.FirstOrDefault(m => m.PropertyName == nameof(Test1Model.Name));

        Assert.AreEqual("ID-nummer", idMeta?.DisplayName);
        Assert.AreEqual("Identifierar objektet.", idMeta?.Description);

        Assert.AreEqual("Namn", idName?.DisplayName);
        Assert.AreEqual("Objektets namn.", idName?.Description);

	}


	[TestMethod]
	public void Model1EnTest()
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

		var lingo = this.CreateLingo();
		var metadata = this.GetMetadataForType<Test1Model>(lingo);

		Assert.AreEqual("Test Model 1", metadata.DisplayName);

		var idMeta = metadata.Properties.FirstOrDefault(m => m.PropertyName == nameof(Test1Model.Id));
		var nameMeta = metadata.Properties.FirstOrDefault(m => m.PropertyName == nameof(Test1Model.Name));

		Assert.AreEqual("ID Number", idMeta?.DisplayName);
		Assert.AreEqual("Identifies the object.", idMeta?.Description);

		Assert.AreEqual("The Name", nameMeta?.DisplayName);
		Assert.AreEqual("Name of the object.", nameMeta?.Description);
	}


	[TestMethod]
	public void Model2Test()
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("sv");

		var lingo = this.CreateLingo();
		var metadata = this.GetMetadataForType<Test2Model>(lingo);

		Assert.AreEqual("Testmodel 2", metadata.DisplayName);

		var idMeta = metadata.Properties.FirstOrDefault(m => m.PropertyName == nameof(Test1Model.Id));
		var nameMeta = metadata.Properties.FirstOrDefault(m => m.PropertyName == nameof(Test1Model.Name));
		var emailMeta = metadata.Properties.FirstOrDefault(m => m.PropertyName == nameof(Test1Model.Email));

		Assert.AreEqual("ID-nummer", idMeta?.DisplayName);
		Assert.AreEqual("Identifierar objektet.", idMeta?.Description);

		Assert.AreEqual("Specialnamn", nameMeta?.DisplayName);
		Assert.AreEqual("Objektets specialnamn.", nameMeta?.Description);

		Assert.AreEqual("Epostadress", emailMeta?.DisplayName);
		Assert.AreEqual("Objektets epostadress.", emailMeta?.Description);
	}


	[TestMethod]
	public void EnumWithoutPrefixAttrTest()
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("sv");

		var lingo = this.CreateLingo();
		var metadata = this.GetMetadataForType<Test1Enum>(lingo);

		Assert.AreEqual("Testalt. 1", metadata.DisplayName);

		var displayNameUndefined = metadata.EnumGroupedDisplayNamesAndValues?.First(kvp => kvp.Value == ((Int32)Test1Enum.Undefined).ToString()).Key.Name;

		Assert.AreEqual("Odefinierat", displayNameUndefined);
	}


	[TestMethod]
	public void EnumWithPrefixAttrTest()
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("sv");

		var lingo = this.CreateLingo();
		var metadata = this.GetMetadataForType<Test2Enum>(lingo);

		Assert.AreEqual("Testalt. 2", metadata.DisplayName);

		var displayNameUndefined = metadata.EnumGroupedDisplayNamesAndValues?.First(kvp => kvp.Value == ((Int32)Test2Enum.Undefined).ToString()).Key.Name;

		Assert.AreEqual("Odefinierat", displayNameUndefined);
	}
}