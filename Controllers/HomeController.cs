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
using FhirClient.Models;

namespace FhirClient.Controllers 
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;
        private readonly Models.IRepository _repo;
        private string _uploadDir = "uploadedFiles";

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
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Dropzone() => View();

        [HttpPost]
        public IActionResult Dropzone(IFormFile file)
        {
            HelperLibrary.WebHelper.IFormFileToFile(file, Path.Combine(_webHostEnvironment.WebRootPath, _uploadDir));
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult UploadHandling()
        {
            var fileInfos = HelperLibrary.FileHelper.GetFileInfoFromDirectory(Path.Combine(_webHostEnvironment.WebRootPath, _uploadDir));
            try
            {
                foreach (var item in fileInfos)
                {
                    if (item.FullName.Substring(item.FullName.Length - 4, 4) == ".xml")
                    {
                        _repo.CreateResource(System.IO.File.ReadAllText(item.FullName).ToFhirBaseFromXml() as Resource);
                    }
                    if (item.FullName.Substring(item.FullName.Length - 5, 5) == ".json")
                    {
                        _repo.CreateResource(System.IO.File.ReadAllText(item.FullName).ToFhirBaseFromJson() as Resource);
                    }

                    // now delete files anyway
                    item.Delete();
                }
                // TODO ApiResponse / success page xDE
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult Privacy() => View();

    }
}
