using DummyDataGen.Models.uDummyNodeModel;
using DummyDataGen.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Web.Common.Controllers;

namespace DummyDataGen.Controllers
{
    [Area("dummy")]
    public class DummyContentController : UmbracoAuthorizedController
    {
        private readonly IuDummyNodeService _uDummyNodesCreateService;
        public DummyContentController(IuDummyNodeService uDummyNodesCreateService)
        {
            _uDummyNodesCreateService = uDummyNodesCreateService;
        }
        [HttpPost]
        public ActionResult CreateNodeWithDummyContents([FromBody] UDummyNodeCreationModel vm)
        {
            try
            {
                if (vm != null)
                {
                    var url = _uDummyNodesCreateService.CreateDummyNode(vm);
                    return new JsonResult(new
                    {
                        Data = url,
                        JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet
                    });

                }
            }
            catch (Exception ex)
            {

            }

            return new JsonResult(new
            {
                Data = string.Empty,
                JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet
            });
        }
    }
}
