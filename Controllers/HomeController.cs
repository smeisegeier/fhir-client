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

        /// <summary>
        /// Controller is recreated after every callback into actions?!
        /// </summary>
        /// <param name="webHost"></param>
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

        public IActionResult PatientListByMe()
        {
            return View(new PatientListViewmodel(_repo.GetPatientsByMe()));
        }

        [HttpGet]
        public IActionResult ObservationList()
        {
            return View(new ObservationListViewmodel(_repo.GetObservations()));
        }

        [HttpGet]
        public IActionResult PatientCreate()
        {
            return View(new PatientEditViewmodel(_repo.CreatePatient()));
        }

        [HttpGet]
        public IActionResult PatientEdit(string id)
        {
            //var pat = _repo.GetPatientById(id);
            var pat = _repo.GetResourceById(id, typeof(Patient));
            if (pat is null)
                return BadRequest();
            else
                return View(new PatientEditViewmodel((Patient)pat));
        }

        // TODO why not Viewmodel?
        [HttpPost]
        //public IActionResult PatientEdit(PatientEditViewmodel patientEditViewmodel)
        public IActionResult PatientEdit(Patient patient)
        {
            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Keys.SelectMany(key => ModelState[key].Errors);
                string s = "Errors: ";
                foreach (var item in modelStateErrors)
                {
                    s += ($"\n" + item.ErrorMessage);
                }
                return Content(s);
            }

            //var patient = patientEditViewmodel._patient;
            var pat = _repo.UpdatePatient(patient);
            var json = _repo.GetJson(patient);
            var link = "<br /><a href=\"/Home/PatientList\">Back to List</a>";
            var content = Content($"The Patient {pat} was successfully updated/edited.\n{json}{link}");
            content.ContentType = "text/html; charset=UTF-8";
            return content;
        }

        public IActionResult PatientDelete(string id)
        {
            return Content(_repo.DeletePatient(id));
        }

        [HttpGet]
        public IActionResult ObservationEdit(string id)
        {
            return View(new ObservationEditViewmodel(_repo.GetObservationById(id)));
        }

        [HttpGet]
        public string ToJson(string id)
        {
            // TODO bad code
            return _repo.GetJson(_repo.GetResourceById(id, typeof(Patient)));
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
            string uploadDir = Path.Combine(webHostEnvironment.WebRootPath + @"\files");
            string fileName = Guid.NewGuid().ToString() + ".json";
            string filePath = Path.Combine(uploadDir, fileName);
            System.IO.File.WriteAllText(filePath, txt);
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

    }
}
