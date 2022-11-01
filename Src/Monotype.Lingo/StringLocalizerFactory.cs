using System;

using Microsoft.Extensions.Localization;

namespace Monotype.Localization
{
    public class StringLocalizerFactory : IStringLocalizerFactory
    {
        public StringLocalizerFactory(Lingo lingo)
        {
            Lingo = lingo;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new StringLocalizer(Lingo.GetTranslator());
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new StringLocalizer(Lingo.GetTranslator());
        }


        private readonly Lingo Lingo;
    }
}
