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
        public CodeableConcept Code { get { return _observation.Code; } }
    }
}
