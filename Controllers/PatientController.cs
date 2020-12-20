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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FhirClient.Controllers
{
    public class PatientController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;
        private readonly Models.IRepository _repo;

        public PatientController(IWebHostEnvironment webHost, Models.IRepository repo)
        {
            _webHostEnvironment = webHost;
            _repo = repo;
        }


        [HttpGet]
        public IActionResult Grid()
        {
            return View(new PatientListViewmodel(_repo.GetPatients()));
        }

        public IActionResult GridFromMe()
        {
            return View("Grid", new PatientListViewmodel(_repo.GetPatientsByMe()));
        }


        [HttpGet]
        public IActionResult Create()
        {
            //var pat = _repo.CreatePatient();
            //return RedirectToAction("Grid");
            return View("Edit", new PatientEditViewmodel(_repo.CreatePatient()));
        }

        [HttpGet]
        public IActionResult Edit(string id)
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
        public IActionResult Edit(Patient patient, string submit)
        //public IActionResult Edit(PatientEditViewmodel patientEditViewmodel, string submit)
        // w/o parameterless ctor, there is no proper viewmodel on submit. Why? 
        // -> That's normal. The default model binder can no longer instantiate your view model as it doesn't have a parameterless constructor. 
        {
            bool success = false;
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
            return View("Result", new PatientResultViewmodel(patient.Id, success, list, outcome));
        }

        public IActionResult Delete(string id)
        {
            _repo.DeletePatient(id);
            return RedirectToAction("Grid");
        }

    }
}
