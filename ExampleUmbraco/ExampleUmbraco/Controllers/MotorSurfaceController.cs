using Examine;
using Examine.LuceneEngine.SearchCriteria;
using Examine.SearchCriteria;
using ExampleUmbraco.App_Start;
using Service.Interfaces;
using Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using umbraco.MacroEngines;
using Umbraco.Web.Mvc;

namespace ExampleUmbraco.Controllers
{
    public class MotorSurfaceController : SurfaceController
    {
        private readonly IMotorService _motorService;

        public MotorSurfaceController()
        {
            _motorService = AppServiceLocator.GetMotorService();
        }

        /// <summary>
        ///     For Custom Index Search using Examine
        /// </summary>
        [ChildActionOnly]
        [HttpGet]
        public ActionResult SearchMotor(string keyword)
        {
            var searchResults = new List<MotorViewModel>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // Use Custom Search exactly which declare in "ExamineSettings.config"
                var searcher = ExamineManager.Instance.SearchProviderCollection["MotorSearcher"];
                var searchCriteria = searcher.CreateSearchCriteria(BooleanOperation.Or);

                // Build query
                var query = searchCriteria
                    .GroupedOr(new[] { "name", "description" }, keyword.Trim().ToLower().MultipleCharacterWildcard())
                    .Compile();

                // Execute search on Index file
                var results = searcher.Search(query);

                // Convert raw data of Examine to MotorViewModel
                foreach (var hit in results)
                {
                    string idStr = hit.Fields.ContainsKey("motorId") ? hit.Fields["motorId"] : string.Empty;
                    if (Guid.TryParse(idStr, out Guid parsedId))
                    {
                        searchResults.Add(new MotorViewModel
                        {
                            Id = parsedId,
                            Name = hit.Fields.ContainsKey("name") ? hit.Fields["name"] : string.Empty,
                            Description = hit.Fields.ContainsKey("description") ? hit.Fields["description"] : string.Empty,
                            Price = hit.Fields.ContainsKey("price") && int.TryParse(hit.Fields["price"], out int price) ? price : 0,
                            ImageUrl = hit.Fields.ContainsKey("imageUrl") ? hit.Fields["imageUrl"] : string.Empty
                        });
                    }
                }
            }
            else
            {
                searchResults = _motorService.GetAll().ToList();
            }

            return PartialView("~/Views/Partials/MotorCardList.cshtml", searchResults);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMotor(MotorViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                _motorService.Add(model);
            }

            ExamineManager.Instance.IndexProviderCollection["MotorIndexer"].RebuildIndex();
            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMotor(MotorViewModel model)
        {
            if (ModelState.IsValid)
            {
                _motorService.Update(model);
            }

            ExamineManager.Instance.IndexProviderCollection["MotorIndexer"].RebuildIndex();
            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMotor(Guid id)
        {
            _motorService.Delete(id);

            ExamineManager.Instance.IndexProviderCollection["MotorIndexer"].RebuildIndex();
            return RedirectToCurrentUmbracoPage();
        }
    }
}