using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;

namespace ExampleUmbraco.Extension.UmbracoConverter
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
    public class RelatedLinkConverter : IUmbracoPropertyConverter<RelatedLink>, IUmbracoPropertyConverter
    {
        public RelatedLink Convert(object rawValue)
        {
            if (rawValue == null) return null;
            try
            {
                // rawValue can be JArray or raw JSON string
                var json = rawValue.ToString().Trim();

                // RelatedLinks Umbraco return JSON array, get first item
                var token = json.StartsWith("[")
                    ? JArray.Parse(json).FirstOrDefault()
                    : JToken.Parse(json);

                return token != null ? RelatedLink.FromToken(token) : null;
            }
            catch { return null; }
        }

        // Non-generic bridge cho extension dùng reflection
        public object ConvertRaw(object rawValue) => Convert(rawValue);
    }

    public class RelatedLinkListConverter : IUmbracoPropertyConverter<List<RelatedLink>>, IUmbracoPropertyConverter
    {
        public List<RelatedLink> Convert(object rawValue)
        {
            if (rawValue == null) return new List<RelatedLink>();
            try
            {
                // rawValue can be JArray or raw JSON string
                var json = rawValue.ToString().Trim();
                if (string.IsNullOrWhiteSpace(json)) return new List<RelatedLink>();

                var array = JArray.Parse(json);
                return array.Select(t => RelatedLink.FromToken(t)).ToList();
            }
            catch { return new List<RelatedLink>(); }
        }

        public object ConvertRaw(object rawValue) => Convert(rawValue);
    }
}