using System;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Extensions.Localization;

namespace Monotype.Localization
{
    public class StringLocalizer : IStringLocalizer
    {
        // Indexers
        public LocalizedString this[string name] => new LocalizedString(name, this.Translator[name]);

        public LocalizedString this[string name, params object[] args] => new LocalizedString(name, this.Translator[name, args]);


        // Constructors
        public StringLocalizer(Translator translator)
        {
			this.Translator = translator;
        }



        // Methods
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }


        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }



        #region Protectede Area

        private readonly Translator Translator;

        #endregion

    }
}
