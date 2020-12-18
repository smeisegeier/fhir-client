using FhirClient.Viewmodels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Controllers
{
    public class ObservationController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;
        private Models.IRepository _repo;

        /// <summary>
        /// Controller is recreated after every callback into actions?!
        /// </summary>
        /// <param name="webHost"></param>
        public ObservationController(IWebHostEnvironment webHost, Models.IRepository repo)
        {
            _webHostEnvironment = webHost;
            _repo = repo;
        }

        public IActionResult Index() 
        {
            return View();
        }

        [HttpGet]
        public IActionResult ObservationEdit(string id)
        {
            return View(new ObservationEditViewmodel(_repo.GetObservation(id)));
        }

        [HttpGet]
        public IActionResult ObservationList()
        {
            return View(new ObservationListViewmodel(_repo.GetObservations()));
        }
    }
}
