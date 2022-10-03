using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace LingoTests;

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
