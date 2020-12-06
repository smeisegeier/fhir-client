using Hl7.Fhir.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Viewmodels
{
    public class DetailsViewmodel
    {
        public Patient patient { get; private set; }
        public string response { get; private set; }
        public string patientJson { get; private set; }

        public DetailsViewmodel(string res) 
        {
            response = res;
            /*
            var mySettings = new JsonSerializerSettings();
            mySettings.NullValueHandling = NullValueHandling.Ignore;
            patient = JsonConvert.DeserializeObject<Patient>(res, mySettings);
            patientJson = JsonConvert.SerializeObject(patient, mySettings);
            */
        }
    }
}
