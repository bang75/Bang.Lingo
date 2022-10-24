using Monotype.Lingo;
using Monotype.Lingo.Extensions;

namespace LingoTests.UnitTests;

[TestClass]
public class BasicTests
{
    [TestMethod]
    public void InitMissingXmlTest()
    {
		var options = new LingoOptions();

		options.AddTranslationXml("i18n/Translations-1.sv.xml", throwIfNotExists: true);
		options.AddTranslationXml("i18n/Translations-1.xx.xml", throwIfNotExists: true);
        
		var lingo = new Lingo(options);

        Assert.ThrowsException<ArgumentException>(() => lingo.Load());
    }

	public void InitWithMissingFolderTest()
	{
		var options = new LingoOptions();

		options.AddTranslationXml("i18n");
		options.AddTranslationXml("translations");

		var lingo = new Lingo(options);

		Assert.ThrowsException<ArgumentException>(() => lingo.Load());
	}


	[TestMethod]
    public void InitWithOptionsTest()
    {
        var options = new LingoOptions()
        {
            MissingItemText = "MissingText: {language}/{key}"
        };

        options.AddParameter("P1", (language, key) => "Param1");

		options.AddTranslationXml("i18n/Translations-1.sv.xml");
		options.AddTranslationXml("i18n/Translations-1.en.xml");

        var lingo = new Lingo(options);

        lingo.Load();

        var i18n = lingo.GetTranslator("sv");

        Assert.AreEqual("Svensktext 1", i18n["Globals.Text1"]);
        Assert.AreEqual("MissingText: sv/MissingText1", i18n["MissingText1"]);

        Assert.AreEqual("Param1", i18n["Parameter1"]);
    }


	[TestMethod]
	public void InitWithFolderTest()
	{
		var options = new LingoOptions();

		options.AddTranslationXml("i18n");

		var lingo = new Lingo(options);

		lingo.Load();


		var dictSv = lingo.GetDictionary("sv");
		var dictEn = lingo.GetDictionary("en");

		Assert.IsTrue(dictSv.NoOfTranslations > 2);
		Assert.AreEqual(2, dictSv.GetTranslations("Globals").Count);

		Assert.IsTrue(dictEn.NoOfTranslations > 2);
		Assert.AreEqual(2, dictEn.GetTranslations("Globals").Count);

		Assert.AreEqual(0, lingo.GetDictionary("xx").NoOfTranslations);
	}


	[TestMethod]
	public void InitWithEmbeddedTest()
	{
		var options = new LingoOptions();

		options.AddTranslationXml(this.GetType().Assembly, "/i18nEmbedded/Translations.xml");

		var lingo = new Lingo(options);

		lingo.Load();


		var dictSv = lingo.GetDictionary("sv");

		Assert.IsTrue(dictSv.NoOfTranslations > 2);
		Assert.AreEqual(2, dictSv.GetTranslations("Globals").Count);
	}

	[TestMethod]
	public void InitWithEmbeddedFolderTest()
	{
		var options = new LingoOptions();

		options.AddTranslationXml(this.GetType().Assembly, "/i18nEmbedded");

		var lingo = new Lingo(options);

		lingo.Load();


		var dictSv = lingo.GetDictionary("sv");

		Assert.IsTrue(dictSv.NoOfTranslations > 2);
		Assert.AreEqual(2, dictSv.GetTranslations("Globals").Count);
	}

	[TestMethod]
	public void InitWithMissingEmbeddedTest()
	{
		var options = new LingoOptions();

		options.AddTranslationXml(this.GetType().Assembly, "/i18n");

		var lingo = new Lingo(options);

		Assert.ThrowsException<ArgumentException>(() => lingo.Load());
	}


	[TestMethod]
    public void InitSuccessfullyTest()
    {
		var options = new LingoOptions();

		options.AddTranslationXml("i18n/Translations-1.sv.xml");
		options.AddTranslationXml("i18n/Translations-1.en.xml");
		options.AddTranslationXml("i18n/Translations-2.sv.xml", throwIfNotExists: true);
		options.AddTranslationXml("i18n/Translations-2.en.xml", throwIfNotExists: true);

		options.AddTranslationXml("i18n/Translations-1.xx.xml", throwIfNotExists: false);

		var lingo = new Lingo(options);

        lingo.Load();


        var dictSv = lingo.GetDictionary("sv");
        var dictEn = lingo.GetDictionary("en");

        Assert.IsTrue(dictSv.NoOfTranslations > 2);
        Assert.AreEqual(2, dictSv.GetTranslations("Globals").Count);

        Assert.IsTrue(dictEn.NoOfTranslations > 2);
        Assert.AreEqual(2, dictEn.GetTranslations("Globals").Count);

        Assert.AreEqual(0, lingo.GetDictionary("xx").NoOfTranslations);


        var i18nSv = lingo.GetTranslator("sv");
        var i18nEn = lingo.GetTranslator("en");

        Assert.IsNotNull(i18nSv.Translate("Globals.Text1", nullIfNotExists: true));
        Assert.IsNull(i18nSv.Translate("Unique.Text1", nullIfNotExists: true));
        Assert.IsNotNull(i18nSv.Translate("Unik.Text1", nullIfNotExists: true));

        Assert.IsNotNull(i18nEn.Translate("Globals.Text1", nullIfNotExists: true));
        Assert.IsNull(i18nEn.Translate("Unik.Text1", nullIfNotExists: true));
        Assert.IsNotNull(i18nEn.Translate("Unique.Text1", nullIfNotExists: true));
    }

}