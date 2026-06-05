using ExampleUmbraco.Extension.UmbracoMapping.UmbracoConverter;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;

namespace ExampleUmbraco.Extension.UmbracoMapping.AdvanceModels
{
    public class RelatedLink
    {
        public string Caption { get; set; }
        public string Link { get; set; }
        public bool NewWindow { get; set; }
        public bool IsInternal { get; set; }

        // For html tag Url in view
        public string Url { get; set; }

        /// <summary>Parse from 1 JToken (1 item in array)</summary>
        public static RelatedLink FromToken(JToken item)
        {
            var caption = item["caption"]?.ToString() ?? "";
            var link = item["link"]?.ToString() ?? "";
            var newWindow = item["newWindow"]?.ToObject<bool>() ?? false;
            var isInternal = item["isInternal"]?.ToObject<bool>() ?? false;

            string resolvedUrl = link;
            if (isInternal && !string.IsNullOrEmpty(link))
            {
                // Call Umbraco converter to help transalate pure internal link after parse JSON
                var umbracoHelper = new UmbracoHelper(Umbraco.Web.UmbracoContext.Current);
                var node = umbracoHelper.TypedContent(link);
                if (node != null) resolvedUrl = node.Url;
            }

            return new RelatedLink
            {
                Caption = caption,
                Link = link,
                IsInternal = isInternal,
                NewWindow = newWindow,
                Url = resolvedUrl
            };
        }
    }

    // ─── Converter for 1 RelatedLink ────────────────────────────────────────
    public class RelatedLinkConverter : VortoAwareRelatedLinkConverter, IUmbracoPropertyConverter<RelatedLink>
    {
        public RelatedLink Convert(object rawValue) => (RelatedLink)ConvertRaw(rawValue);
        protected override object ConvertJson(string cleanJson)
        {
            try
            {
                var token = cleanJson.TrimStart().StartsWith("[")
                    ? JArray.Parse(cleanJson).FirstOrDefault()
                    : JToken.Parse(cleanJson);

                return token != null ? RelatedLink.FromToken(token) : null;
            }
            catch { return null; }
        }

        protected override object GetDefault() => null;
    }

    public class RelatedLinkListConverter : VortoAwareRelatedLinkConverter, IUmbracoPropertyConverter<List<RelatedLink>>
    {
        public List<RelatedLink> Convert(object rawValue) => (List<RelatedLink>)ConvertRaw(rawValue);

        protected override object ConvertJson(string cleanJson)
        {
            try
            {
                return JArray.Parse(cleanJson)
                             .Select(t => RelatedLink.FromToken(t))
                             .ToList();
            }
            catch { return GetDefault(); }
        }

        protected override object GetDefault() => new List<RelatedLink>();
    }
}