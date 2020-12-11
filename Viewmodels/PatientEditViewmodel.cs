using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Viewmodels
{
    public class PatientEditViewmodel
    {
        private Patient _patient;

        public PatientEditViewmodel(Patient pat)
        {
            _patient = pat;
        }

        public string Id { get { return _patient.Id; } }
        public bool? Active { get { return _patient.Active; } }

        public List<Identifier> Identifier { get { return _patient.Identifier; } }

        public AdministrativeGender? Gender { get { return _patient.Gender; } }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime? BirthDate { get { return IsoToDateTime(_patient.BirthDate); } }

        public List<HumanName> Name { get { return _patient.Name; } }
        public List<Address> Address { get { return _patient.Address; } }

        public CodeableConcept MaritalStatus { get { return _patient.MaritalStatus; } }

        public Hl7.Fhir.Model.DataType Deceased { get { return _patient.Deceased; } }

        public FhirBoolean deceasedBoolean { get { return _patient.Deceased?.GetType() == typeof(FhirBoolean)? (FhirBoolean)_patient.Deceased : null; } }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime? deceasedDateTime { get { return _patient.Deceased?.GetType() == typeof(FhirDateTime)? IsoToDateTime(((FhirDateTime)_patient.Deceased).Value) : null; } }
        //public FhirDateTime deceasedDateTime { get { return _patient.Deceased?.GetType() == typeof(FhirDateTime)? (FhirDateTime)_patient.Deceased: null; } }

        public List<Patient.ContactComponent> Contact { get { return _patient.Contact; } }

        public List<Patient.CommunicationComponent> Communication{ get { return _patient.Communication; } }

        public List<ResourceReference> GeneralPractitioner { get { return _patient.GeneralPractitioner; } }

        public DateTime? IsoToDateTime(string date)
        {
            if (string.IsNullOrEmpty(date))
                return null;
            if (date.Length == 8)
                date = string.Concat(date, "T00:00:00Z");
            return DateTime.Parse(date, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }
    }
}
