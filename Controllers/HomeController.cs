using FhirClient.Viewmodels;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
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
        public IActionResult PatientList()
        {
            return View(new PatientListViewmodel(_repo.GetPatients()));
        }

        [HttpGet]
        public IActionResult ObservationList()
        {
            return View(new ObservationListViewmodel(_repo.GetObservations()));
        }


        [HttpGet]
        public IActionResult PatientEdit(string id)
        {
            return View(new PatientEditViewmodel(_repo.GetPatientFromServerById(id)));
        }

        [HttpGet]
        public IActionResult ObservationEdit(string id)
        {
            return View(new ObservationEditViewmodel(_repo.GetObservationFromServerById(id)));
        }

        [HttpGet]
        public string ToJson(string id)
        {
            // TODO bad code
            return _repo.GetJson(_repo.GetPatientFromServerById(id));
        }

        [HttpGet]
        public string ToJsonObs(string id)
        {
            // TODO bad code
            return _repo.GetJsonObs(_repo.GetObservationFromServerById(id));
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



        //TODO Delete: only my own


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
