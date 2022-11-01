#nullable enable

using System.Xml;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace Monotype.Localization.Extensions;

public static class LingoOptionsExtensions
{
	public static void AddTranslationXml(this LingoOptions options, String path, Boolean throwIfNotExists = true)
	{
		options.LoadTranslations += lingo =>
		{
			var physicalPath = path;

			if(Directory.Exists(physicalPath))
			{
				foreach(var file in Directory.GetFiles(physicalPath, "*.xml", SearchOption.AllDirectories))
				{
					using var fileStream = File.OpenRead(file);

					LoadXml(lingo, fileStream);
				}
			}
			else if(File.Exists(physicalPath))
			{
				using var fileStream = File.OpenRead(path);

				LoadXml(lingo, fileStream);
			}
			else if(throwIfNotExists)
			{
				throw new ArgumentException($"Could not find path '{path}'");
			}
		};
	}

	public static void AddTranslationXml(this LingoOptions options, IFileProvider fileProvider,  String path, Boolean throwIfNotExists = true)
	{
		options.LoadTranslations += lingo =>
		{
			void loadXmlFile(IFileInfo fileInfo)
			{
				using var fileStream = fileInfo.CreateReadStream();

				LoadXml(lingo, fileStream);
			};

			Int32 loadXmlDirectory(String folderPath, Int32 noOfFiles = 0)
			{
				foreach(var fileInfo in fileProvider.GetDirectoryContents(folderPath))
				{
					if(fileInfo.IsDirectory)
					{
						noOfFiles += loadXmlDirectory(folderPath.UnSuffix("/") + fileInfo.Name.Prefix("/"));
					}
					else
					{
						noOfFiles++;
						loadXmlFile(fileInfo);
					}
				}

				return noOfFiles;
			};

			if(path.IsSuffixed(".xml"))
			{
				var fileInfo = fileProvider.GetFileInfo(path);

				if(fileInfo.Exists)
				{
					loadXmlFile(fileInfo);
					return;
				}
			}
			else
			{
				if(loadXmlDirectory(path) > 0)
				{
					return;
				}
			}

			if(throwIfNotExists)
			{
				throw new ArgumentException($"Could not find path '{path}'");
			}
		};
	}


	public static void AddTranslationXml(this LingoOptions options, Assembly assembly, String path, Boolean throwIfNotExists = true)
	{
		options.LoadTranslations += lingo =>
		{
			var resCount = 0;
			var resPattern = path.Replace("/", ".");
			var strComp = StringComparison.OrdinalIgnoreCase;

			if(resPattern.StartsWith('.'))
			{
				var rootNs = assembly.ExportedTypes
					.Select(t => t.Namespace?.Split(".").FirstOrDefault())
					.Where(t => t != null)
					.GroupBy(ns => ns)
					.OrderByDescending(g => g.Count())
					.FirstOrDefault()?
					.Key;

				resPattern = $"{rootNs}{resPattern}";
			}

			if(!resPattern.EndsWith(".xml", strComp))
			{
				resPattern = resPattern.Suffix(".")!;
			}

			foreach(var resId in assembly.GetManifestResourceNames()
				.Where(r => r.StartsWith(resPattern, strComp) && r.EndsWith(".xml", strComp)))
			{
				var stream = assembly.GetManifestResourceStream(resId);

				if(stream != null)
				{
					resCount++;
					LoadXml(lingo, stream);
				}
			}

			if(resCount == 0 && throwIfNotExists)
			{
				throw new ArgumentException($"Could not find resource '{resPattern}'");
			}
		};
	}



	#region Protected Area

	private static void LoadXml(Lingo lingo, Stream stream)
	{
		var doc = new XmlDocument();

		doc.Load(stream);

		static void ReadXml(TranslationDictionary dictionary, XmlNodeList nodes, String? key = null)
		{
			if(nodes.Count > 0)
			{
				foreach(XmlNode node in nodes)
				{
					switch(node.NodeType)
					{
						case XmlNodeType.Text:
							var text = node.Value;

							var markup = node.ParentNode?.Attributes?["Markup"]?.Value;
							var textFormat = markup != null ? (markup.Equals("Paragraphed", StringComparison.OrdinalIgnoreCase) ? TextFormat.ParagraphedMarkup : TextFormat.Markup) : TextFormat.Plain;

							if(key != null)
							{
								dictionary.Add(key, text ?? String.Empty, textFormat, replaceIfExists: true);
							}
							break;

						case XmlNodeType.Element:
							ReadXml(dictionary, node.ChildNodes, String.Format("{0}{1}{2}", key, key != null ? "." : null, node.Name));
							break;
					}
				}
			}
			else if(key != null)
			{
				dictionary.Add(key, String.Empty);
			}
		}

		foreach(XmlNode node in doc.GetElementsByTagName("Translations"))
		{
			if(node != null)
			{
				var lang = node.Attributes?.GetNamedItem("Language")?.Value;

				if(lang != null)
				{
					ReadXml(lingo.GetDictionary(lang), node.ChildNodes);
				}
			}
		}
	}

	#endregion

}