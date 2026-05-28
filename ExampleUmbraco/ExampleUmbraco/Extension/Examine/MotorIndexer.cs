using Examine.LuceneEngine.Providers;
using ExampleUmbraco.App_Start;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Xml.Linq;

namespace ExampleUmbraco.Extension.Examine
{
    public class MotorIndexer : LuceneIndexer
    {
        // Constructor without params (for Examine use to create from config XML)
        public MotorIndexer() { }

        /// <summary>
        ///     Load all data of a type to index
        /// </summary>
        protected override void PerformIndexAll(string type)
        {
            // Kiểm tra để đảm bảo chỉ xử lý đúng type là "Motor"
            if (type.Equals("Motor", StringComparison.InvariantCultureIgnoreCase))
            {
                var motorService = AppServiceLocator.GetMotorService();
                if (motorService == null) return;

                var allMotors = motorService.GetAll();

                var xmlNodes = new List<XElement>();

                foreach (var motor in allMotors)
                {
                    // Encapsulate data into XML structure.
                    var node = new XElement("node",
                        new XAttribute("id", Math.Abs(motor.Id.GetHashCode())),
                        new XAttribute("nodeTypeAlias", type),

                        // Map data match XML in <IndexUserFields> in file ExamineIndex.config
                        new XElement("motorId", motor.Id.ToString()),
                        new XElement("name", motor.Name ?? string.Empty),
                        new XElement("description", motor.Description ?? string.Empty),
                        new XElement("price", motor.Price.ToString(CultureInfo.InvariantCulture)),
                        new XElement("imageUrl", motor.ImageUrl ?? string.Empty)
                    );

                    xmlNodes.Add(node);
                }

                // Add this XML list into Lucene to create index file
                AddNodesToIndex(xmlNodes, type);
            }
        }

        /// <summary>
        ///     This method rebuild all index in Backoffice or when call
        ///     <b>ExamineManager.Instance.IndexProviderCollection["MotorIndexer"].RebuildIndex()</b>
        /// </summary>
        protected override void PerformIndexRebuild()
        {
            // Old index will be deleted automatically by Examine befor call into here
            PerformIndexAll("Motor");
        }
    }
}