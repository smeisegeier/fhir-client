using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Controllers
{
    public class OrganizationController : Controller
    {

        private IWebHostEnvironment _webHostEnvironment;
        private readonly Models.IRepository _repo;

        /// <summary>
        /// Controller is recreated after every callback into actions?!
        /// </summary>
        /// <param name="webHost"></param>
        public OrganizationController(IWebHostEnvironment webHost, Models.IRepository repo)
        {
            _webHostEnvironment = webHost;
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Grid() => View(_repo.GetOrganizations());

        [HttpGet]
        public IActionResult Edit(string id) => View(_repo.GetOrganization(id));

        [HttpGet]
        public IActionResult ToJson(string id) => new JsonResult(_repo.GetOrganization(id));

        [HttpGet]
        public IActionResult ToXml(string id) => Content(_repo.GetOrganization(id).ToXml());

    }
}
