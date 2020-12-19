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

        [HttpGet]
        public IActionResult PatientList()
        {
            return View(new PatientListViewmodel(_repo.GetPatients()));
        }

        public IActionResult PatientListByMe()
        {
            return View("PatientList", new PatientListViewmodel(_repo.GetPatientsByMe()));
        }


        [HttpGet]
        public IActionResult PatientCreate()
        {
            //var pat = _repo.CreatePatient();
            //return RedirectToAction("PatientList");
            return View("PatientEdit",new PatientEditViewmodel(_repo.CreatePatient()));
        }

        [HttpGet]
        public IActionResult PatientEdit(string id)
        {
            var pat = _repo.GetPatient(id);
            if (pat is null)
                return BadRequest();
            else
            {                
                return View(new PatientEditViewmodel(pat));
            }
        }

        [HttpGet]
        public string ToJson(string id) => _repo.GetPatientAsJson(id);

        [HttpGet]
        public string ToXml(string id) => _repo.GetPatientAsXml(id);

        /// <summary>
        /// Method is called on PatientEdit submit. Updates patient object. Forwards to Result View
        /// </summary>
        /// <remarks>
        /// Obviously the patient object itself can be obtained on submit from the view. This is strange, because the wrapped Viewmodel is 
        /// populated to the view, not the model object itself. w/e, processing the model seems to avoid some odds (Date vs string, date-picker etc), 
        /// hence the model is used.
        /// </remarks>
        /// <param name="patient">Model object from Viewmodel</param>
        /// <param name="submit">Cancel or Save</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PatientEdit(Patient patient, string submit)
        //public IActionResult PatientEdit(PatientEditViewmodel patientEditViewmodel, string submit)
        // w/o parameterless ctor, there is no proper viewmodel on submit. Why? 
        // -> That's normal. The default model binder can no longer instantiate your view model as it doesn't have a parameterless constructor. 
        {
            bool success=false;
            OperationOutcome outcome = null;
            List<ModelError> list = null;

            switch (submit)
            {
                case "Validate":
                    {
                        //var validator = new Validator();
                        outcome = new Validator().Validate(patient);
                        success = outcome.Success;
                        goto case "Save";
                    }
                case "Save":
                    {
                        if (!ModelState.IsValid)
                        {
                            list = ModelState.Keys.SelectMany(key => ModelState[key].Errors).ToList();
                            break;
                        }

                        var patneu = _repo.UpdatePatient(patient);
                        success = !(patneu is null);
                        break;
                    }
                default:
                    return StatusCode(StatusCodes.Status405MethodNotAllowed);
            }
            return View("PatientResult", new PatientResultViewmodel(patient.Id, success, list, outcome));
        }

        public IActionResult PatientDelete(string id)
        {
            _repo.DeletePatient(id);
            return RedirectToAction("PatientList");
        }



        /// <summary>
        /// Creates a JSON File from string into files folder
        /// </summary>
        /// <param name="txt"></param>
        private void createJsonFile(string txt)
        {
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath + @"\files");
            string fileName = Guid.NewGuid().ToString() + ".json";
            string filePath = Path.Combine(uploadDir, fileName);
            System.IO.File.WriteAllText(filePath, txt);
        }


        /*
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
                //var json = _repo.GetJson(patient);
                //var link = "<br /><a href=\"/Home/PatientList\">Back to List</a>";
                //var content = Content($"The Patient {pat} was successfully updated/edited.\n{json}{link}");
                //content.ContentType = "text/html; charset=UTF-8";
                //return content;
        */
    }
}
