using System.Collections.Generic;

namespace ExampleUmbraco.Extension.UmbracoConverter
{
    public class LayoutNode
    {
        public string TabTitle { get; set; }
        public string TabIcon { get; set; }

        public string MailTo { get; set; }
        public string Telephone { get; set; }

        public string BrandName { get; set; }

        [UmbracoConverter(typeof(RelatedLinkListConverter))]
        public List<RelatedLink> ListNavigators { get; set; }

        public string Address { get; set; }
        public string CopyRight { get; set; }
        public string TitleListProductsRef { get; set; }
        [UmbracoConverter(typeof(RelatedLinkListConverter))]
        public List<RelatedLink> ListProductsRef { get; set; }
        public string TitleListOthersRef { get; set; }
        [UmbracoConverter(typeof(RelatedLinkListConverter))]
        public List<RelatedLink> ListOthersRef { get; set; }
    }
}
