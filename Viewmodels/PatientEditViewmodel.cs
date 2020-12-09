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
        public AdministrativeGender? Gender { get { return _patient.Gender; } }
        public string FamilyName 
        { 
            get 
            { 
                return _patient.Name.FirstOrDefault()?.Family;
            } 
        }
        public string BirthDate { get { return _patient.BirthDate; } }
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime? BirthDateTime { get { return IsoToDateTime(BirthDate); } }
        public bool? Active{ get { return _patient.Active; } }

        public FhirBoolean deceasedBoolean { get { return _patient.Deceased?.GetType() == typeof(FhirBoolean)? (FhirBoolean)_patient.Deceased : null; } }
        public FhirDateTime deceasedDateTime { get { return _patient.Deceased?.GetType() == typeof(FhirDateTime)? (FhirDateTime)_patient.Deceased : null; } }
        public string maritalStatus1sys { get { return _patient.MaritalStatus?.Coding.FirstOrDefault()?.System; } }
        // TODO how to get Codes + Display
        public string maritalStatus1cod { get { return _patient.MaritalStatus?.Coding.FirstOrDefault()?.Code; } }
        public string maritalStatus1dsp { get { return _patient.MaritalStatus?.Coding.FirstOrDefault()?.Display; } }
        public string maritalStatusTxt { get { return _patient.MaritalStatus?.Text; } }
        public string contact1name { get { return _patient.Contact.FirstOrDefault()?.Name.Family; } }
        public string contact1rel1coding1cod { get { return _patient.Contact.FirstOrDefault()?.Relationship.FirstOrDefault()?.Coding.FirstOrDefault()?.Code; } }
        public string communication1languageCoding1code { get { return _patient.Communication.FirstOrDefault()?.Language.Coding.FirstOrDefault()?.Code; } }
        public string communication1languageCoding1system { get { return _patient.Communication.FirstOrDefault()?.Language.Coding.FirstOrDefault()?.System; } }
        public string communication1languageCoding1display { get { return _patient.Communication.FirstOrDefault()?.Language.Coding.FirstOrDefault()?.Display; } }

        public string generalPractitionier1reference { get {return _patient.GeneralPractitioner?.FirstOrDefault()?.Reference; } }
        public string generalPractitionier1display { get { return _patient.GeneralPractitioner?.FirstOrDefault()?.Display; } }


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
