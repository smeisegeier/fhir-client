using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Viewmodels
{
    public class ObservationEditViewmodel
    {
        private Observation _observation;
        public ObservationEditViewmodel(Observation obs)
        {
            _observation = obs;
        }

        public string Id { get { return _observation.Id; } }

        public ObservationStatus? Status { get { return _observation.Status; } }
        public string coding1code { get { return _observation.Code.Coding.FirstOrDefault()?.Code; } }
        public string coding1system { get { return _observation.Code.Coding.FirstOrDefault()?.System; } }
        public string codingText { get { return _observation.Code.Text; } }

    }
}
