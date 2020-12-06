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
        private string _baseUrl = "https://vonk.fire.ly/R4";
        private IWebHostEnvironment webHostEnvironment;

        public HomeController(IWebHostEnvironment webHost)
        {
            webHostEnvironment = webHost;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Receives selected Id
        /// </summary>
        /// <param name="Id">Id of selected item (is bound to html-tag 'name', not 'id'!)</param>
        /// <returns></returns>
        [HttpPost]
        public string Index(int Id)
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                try
                {
                    var pat = client.Read<Patient>("Patient/" + Id);
                    var xd = new Hl7.Fhir.Serialization.FhirJsonSerializer();
                    string patString = xd.SerializeToString(pat);
                    //createJsonFile(patString);
                    //return new JsonResult(patString);
                    // return Json(pat);
                    return patString;
                }
                catch (FhirOperationException)
                {
                    return Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound.ToString();
                }
            }
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


        public JsonResult PostPatient()
        {
            var pat = createTestPatient();
            var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl);
            var result = client.Create(pat);
            return Json(result);
        }

        public IActionResult Details()
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                var pat4 = client.Read<Patient>("Patient/4");
                var xd = new Hl7.Fhir.Serialization.FhirJsonSerializer();
                string pat4String = xd.SerializeToString(pat4);
                createJsonFile(pat4String);
                return View(new DetailsViewmodel(pat4String));
            }
        }

        /// <summary>
        /// Gets data via the GET command.
        /// </summary>
        /// <remarks>
        /// Uses ReSharper package.
        /// </remarks>
        /// <param name="baseUrl"></param>
        /// <returns>RestResponse obj</returns>
        public IRestResponse GetFromUrl(string baseUrl)
        {
            var client = new RestClient(baseUrl);
            var request = new RestRequest();
            var response = client.Get(request);
            return response;
        }

        /// <summary>
        /// Creates a JSON File from string
        /// </summary>
        /// <param name="txt"></param>
        public void createJsonFile(string txt)
        {
            string uploadDir = Path.Combine(webHostEnvironment.WebRootPath + "files");
            string fileName = Guid.NewGuid().ToString() + ".json";
            string filePath = Path.Combine(uploadDir, fileName);
            System.IO.File.WriteAllText(filePath, txt);
        }


        private Patient createTestPatient()
        {
            var MyPatient = new Patient();
            MyPatient.Active = true;
            MyPatient.Identifier.Add(new Identifier() { System = "http://hl7.org/fhir/sid/us-ssn", Value = "000-12-3456" });
            MyPatient.Gender = AdministrativeGender.Male;
            MyPatient.Deceased = new FhirDateTime("2020-04-23");
            MyPatient.Name.Add(new HumanName()
            {
                Use = HumanName.NameUse.Official,
                Family = "Stokes",
                Given = new List<string>() { "Bran", "Deacon" },
                Period = new Period() { Start = "2015-05-12", End = "2020-02-15" }
            });
            MyPatient.AddExtension("http://hl7.org/fhir/StructureDefinition/patient-birthPlace", new Address() { Country = "US" });
            return MyPatient;
        }
    }
}
