using FhirClient.Viewmodels;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Vonk.Fhir.R4;
using Vonk.ElementModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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

namespace FhirClient.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment webHostEnvironment;
        private Models.Repository _repo;

        public HomeController(IWebHostEnvironment webHost)
        {
            webHostEnvironment = webHost;
            _repo = new Models.Repository();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Read()
        {
            return View();
        }

        [HttpGet]
        public IActionResult List()
        {
            return View(new ListViewmodel(_repo.GetPatients()));
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            return View(new EditViewmodel(_repo.GetPatientFromServerById(id)));
        }

        [HttpGet]
        public string ToJson(string id)
        {
            // TODO bad code
            return _repo.GetJson(_repo.GetPatientFromServerById(id));
        }


        /// <summary>
        /// Reads selected Id
        /// </summary>
        /// <param name="Id">Id of selected item (is bound to html-tag 'name', not 'id'!)</param>
        [HttpPost]
        public string Read(string Id)
        {
            return null;
        }



        //// GET api/values/getFromExternal
        //[HttpGet, Route("getFromExternal")]
        // https://derekarends.com/how-to-create-http-request-asp-dotnet-core/
        //public async Task<IActionResult> GetFromExternal()
        //{
        //    using (var response = await _httpClient.GetAsync("https://localhost:5001/api/externalEndpoint"))
        //    {
        //        if (!response.IsSuccessStatusCode)
        //            return StatusCode((int)response.StatusCode);

        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        var deserializedResponse = JsonConvert.DeserializeObject<List<string>>(responseContent);

        //        return Ok(deserializedResponse);
        //    }
        //}


        // TODO Create
        public JsonResult Create()
        {
            return null;
        }

        //TODO Delete: only my own

        public IActionResult Details()
        {
            return View();// (new DetailsViewmodel(pat4String));
            
        }

        /// <summary>
        /// Gets data via the GET command.
        /// </summary>
        /// <remarks>
        /// Uses ReSharper package.
        /// </remarks>
        /// <param name="baseUrl"></param>
        /// <returns>RestResponse obj</returns>
        [Obsolete]
        public IRestResponse GetFromUrl(string baseUrl)
        {
            var client = new RestClient(baseUrl);
            var request = new RestRequest();
            var response = client.Get(request);
            return response;
        }

        /// <summary>
        /// Creates a JSON File from string into files folder
        /// </summary>
        /// <param name="txt"></param>
        private void createJsonFile(string txt)
        {
            string uploadDir = Path.Combine(webHostEnvironment.WebRootPath + "files");
            string fileName = Guid.NewGuid().ToString() + ".json";
            string filePath = Path.Combine(uploadDir, fileName);
            System.IO.File.WriteAllText(filePath, txt);
        }


    }
}
