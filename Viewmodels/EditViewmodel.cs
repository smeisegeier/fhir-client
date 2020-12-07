using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Viewmodels
{
    public class EditViewmodel
    {
        private Patient CurrentPatient { get; set; }

        public EditViewmodel(Patient pat)
        {
            CurrentPatient = pat;
        }

        public string Id { get { return CurrentPatient.Id; } }
        public AdministrativeGender? Gender { get { return CurrentPatient.Gender; } }
        public string FamilyName 
        { 
            get 
            { 
                return CurrentPatient.Name.FirstOrDefault().Family;
            } 
        }
        public string BirthDate { get { return CurrentPatient.BirthDate; } }
        public bool? Active{ get { return CurrentPatient.Active; } }

    }
}
