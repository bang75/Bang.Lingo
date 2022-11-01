#nullable enable

using System.Reflection;

using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Options;

using Monotype.Localization.Extensions;

namespace Monotype.Localization;

internal class ConfigureLingoOptions : IConfigureOptions<LingoOptions>
{
	// Constructors
	public ConfigureLingoOptions(IWebHostEnvironment hostEnv)
	{
		this.HostEnv = hostEnv;
	}

	public void Configure(LingoOptions options)
	{
		var entryAssembly = Assembly.GetEntryAssembly();

		if(entryAssembly != null)
		{
			options.AddTranslationXml(entryAssembly, "/Lingo", throwIfNotExists: false);
		}

		if(this.HostEnv?.ContentRootPath.IsNullOrWhiteSpace() == false)
		{
			if(this.HostEnv?.ContentRootFileProvider != null)
			{
				options.AddTranslationXml(this.HostEnv.ContentRootFileProvider, "Lingo", throwIfNotExists: false);
			}
			else
			{
				options.AddTranslationXml(Path.Combine(this.HostEnv!.ContentRootPath, "Lingo"), throwIfNotExists: false);
			}
		}
	}



	#region Protected Area

	private readonly IWebHostEnvironment HostEnv;

	#endregion

}
