using System.Collections.Generic;

namespace Service.ViewModels.UmbracoContent
{
    public class PlanItemModel
    {
        public string PlanImage { get; set; }
        public string PlanName { get; set; }
        public string PlanTitle { get; set; }
        public string PlanSummary { get; set; }
        public IEnumerable<string> PlanBenefitList { get; set; }
        public string PlanAdvance1Title { get; set; }
        public string PlanAdvance1Content { get; set; }
        public string PlanAdvance2Title { get; set; }
        public string PlanAdvance2Content { get; set; }
        public string PlanAdvance3Title { get; set; }
        public string PlanAdvance3Content { get; set; }
    }
}