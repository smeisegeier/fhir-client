using FhirClient.Viewmodels;
using Hl7.Fhir.Model;
using Hl7.Fhir.Validation;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FhirClient.Controllers 
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;
        private readonly Models.IRepository _repo;

        /// <summary>
        /// Controller is recreated after every callback into actions?!
        /// </summary>
        /// <param name="webHost"></param>
        public HomeController(IWebHostEnvironment webHost, Models.IRepository repo)
        {
            _webHostEnvironment = webHost;
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
