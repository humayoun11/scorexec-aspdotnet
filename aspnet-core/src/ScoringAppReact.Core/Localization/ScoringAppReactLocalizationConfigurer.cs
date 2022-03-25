using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace ScoringAppReact.Localization
{
    public static class ScoringAppReactLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(ScoringAppReactConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(ScoringAppReactLocalizationConfigurer).GetAssembly(),
                        "ScoringAppReact.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
