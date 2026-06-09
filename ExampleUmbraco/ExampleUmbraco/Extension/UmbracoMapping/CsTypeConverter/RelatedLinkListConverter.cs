using Newtonsoft.Json.Linq;
using Service.ViewModels.UmbracoDataType;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using Umbraco.Web;

namespace ExampleUmbraco.Extension.UmbracoMapping.CsTypeConverter
{
    public class RelatedLinkListConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var rawString = value as string;
            if (string.IsNullOrWhiteSpace(rawString)) return new List<RelatedLink>();

            try
            {
                string jsonString = TryUnwrapVorto(rawString) ?? rawString;

                var rawJson = JArray.Parse(jsonString);
                var result = new List<RelatedLink>();

                var umbHelper = new UmbracoHelper(UmbracoContext.Current);

                foreach (JObject item in rawJson)
                {
                    var link = new RelatedLink
                    {
                        Caption = item["caption"]?.ToString(),
                        NewWindow = item["newWindow"]?.Value<bool>() ?? false,
                        IsInternal = item["isInternal"]?.Value<bool>() ?? false,
                        Link = item["link"]?.ToString()
                    };

                    link.Url = link.Link;
                    // Convert if internal link url
                    if (link.IsInternal && !string.IsNullOrEmpty(link.Link))
                    {
                        var linkedNode = umbHelper.TypedContent(link.Link);
                        link.Url = linkedNode != null ? linkedNode.Url : "#";
                    }

                    result.Add(link);
                }

                return result;
            }
            catch
            {
                return new List<RelatedLink>();
            }
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
    }
}