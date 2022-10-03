using Bang.Lingo;
using Bang.Lingo.Extensions;

namespace LingoTests;

[TestClass]
public class BasicTests
{
	[TestMethod]
	public void InitMissingXmlTest()
	{
		var lingo = new Lingo();

		lingo.AddXml("i18n/Translations-1.sv.xml", throwIfNotExists: true);
		lingo.AddXml("i18n/Translations-1.xx.xml", throwIfNotExists: true);

		Assert.ThrowsException<ArgumentException>(()  => lingo.Load());
	}

	[TestMethod]
	public void InitWithOptionsTest()
	{
		var options = new LingoOptions()
		{
			MissingItemText = "MissingText: {language}/{key}"
		};

		options.Parameters.Add("P1", (language, key) => "Param1");

		var lingo = new Lingo(options);

		lingo.AddXml("i18n/Translations-1.sv.xml");
		lingo.AddXml("i18n/Translations-1.en.xml");

		lingo.Load();

		var i18n = lingo.GetTranslator("sv");

		Assert.AreEqual("Svensktext 1", i18n["Globals.Text1"]);
		Assert.AreEqual("MissingText: sv/MissingText1", i18n["MissingText1"]);

		Assert.AreEqual("Param1", i18n["Parameter1"]);
	}

	[TestMethod]
	public void InitSuccessfullyTest()
	{
		var lingo = new Lingo();

		lingo.AddXml("i18n/Translations-1.sv.xml");
		lingo.AddXml("i18n/Translations-1.en.xml");
		lingo.AddXml("i18n/Translations-2.sv.xml", throwIfNotExists: true);
		lingo.AddXml("i18n/Translations-2.en.xml", throwIfNotExists: true);

		lingo.AddXml("i18n/Translations-1.xx.xml", throwIfNotExists: false);

		lingo.Load();


		var dictSv = lingo.GetDictionary("sv");
		var dictEn = lingo.GetDictionary("en");

		Assert.IsTrue(dictSv.NoOfTranslations > 0);
		Assert.AreEqual(2, dictSv.GetTranslations("Globals").Count);

		Assert.IsTrue(dictEn.NoOfTranslations > 0);
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