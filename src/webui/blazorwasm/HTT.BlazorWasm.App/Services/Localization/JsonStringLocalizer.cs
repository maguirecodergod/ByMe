using Microsoft.Extensions.Localization;

namespace HTT.BlazorWasm.App.Services.Localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly string _resourceName;
        private readonly LocalizationService _localizationService;

        public JsonStringLocalizer(string resourceName, LocalizationService localizationService)
        {
            _resourceName = resourceName;
            _localizationService = localizationService;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = _localizationService.GetString(_resourceName, name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = _localizationService.GetString(_resourceName, name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, format == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Enumerable.Empty<LocalizedString>();
        }
    }
}
