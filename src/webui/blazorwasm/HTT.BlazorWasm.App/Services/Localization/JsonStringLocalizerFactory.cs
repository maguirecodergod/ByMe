using Microsoft.Extensions.Localization;

namespace HTT.BlazorWasm.App.Services.Localization
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly LocalizationService _localizationService;

        public JsonStringLocalizerFactory(LocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(resourceSource.Name, _localizationService);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var lastDot = baseName.LastIndexOf('.');
            var name = lastDot >= 0 ? baseName.Substring(lastDot + 1) : baseName;
            return new JsonStringLocalizer(name, _localizationService);
        }
    }
}
