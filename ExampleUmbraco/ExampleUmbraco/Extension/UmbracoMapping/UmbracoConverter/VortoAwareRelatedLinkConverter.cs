using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;

namespace ExampleUmbraco.Extension.UmbracoMapping.UmbracoConverter
{
    public abstract class VortoAwareRelatedLinkConverter : IUmbracoPropertyConverter
    {
        public object ConvertRaw(object rawValue)
        {
            if (rawValue == null) return GetDefault();

            string rawString = rawValue.ToString().Trim();
            if (string.IsNullOrWhiteSpace(rawString)) return GetDefault();

            // Unwrap Vorto JSON if have
            string cleanJson = TryUnwrapVorto(rawString) ?? rawString;

            return ConvertJson(cleanJson);
        }

        private static string TryUnwrapVorto(string raw)
        {
            // Vorto wrapper check
            if (!raw.Contains("\"values\":") || !raw.Contains("\"dtdGuid\":"))
                return null;

            try
            {
                var vortoJson = JObject.Parse(raw);
                var valuesObj = vortoJson["values"] as JObject;
                if (valuesObj == null || valuesObj.Count == 0) return null;

                string currentCulture = Thread.CurrentThread.CurrentUICulture.Name;
                // Priority current thread culture, if null -> fallback get first language found
                var cultureToken = valuesObj[currentCulture]
                                ?? valuesObj.Properties().First().Value;

                return cultureToken?.ToString();
            }
            catch { return null; }
        }

        // Subclass implement this with cleaned JSON (don't have Vorto wrapper)
        protected abstract object ConvertJson(string cleanJson);

        // Data return when null/error
        protected abstract object GetDefault();
    }
}