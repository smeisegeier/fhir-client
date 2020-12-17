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
        // must be visible for controller
        public Patient _patient { get; private set; }

        public PatientEditViewmodel(Patient pat)
        {
            _patient = pat;
        }

        public PatientEditViewmodel()
        {
            _patient = new Patient();
        }


        public string Id { get { return _patient.Id; } }

        public bool? Active
        {
            get
            { 
                return _patient.Active; 
            }
            set
            {
                _patient.Active = value;
            }
        }

        public List<Identifier> Identifier 
        { 
            get 
            { 
                return _patient.Identifier; 
            }
            set
            {
                _patient.Identifier = value;
            }
        }

        public AdministrativeGender? Gender
        { 
            get 
            { 
                return _patient.Gender; 
            }
            set
            {
                _patient.Gender = value;
            }
        }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime? BirthDate 
        { 
            get 
            { 
                return Helper.IsoToDateTime(_patient.BirthDate); 
            } 
            set
            {
                _patient.BirthDate = value.ToString();
            }
        }


        // TODO Given names still wont save
        public List<HumanName> Name 
        { 
            get 
            {
                return _patient.Name; 
            } 
            set
            {
                _patient.Name = value;
            }
        }

        public List<Address> Address 
        { 
            get 
            { 
                return _patient.Address; 
            }
            set
            {
                _patient.Address=value;
            }
        }

        public CodeableConcept MaritalStatus 
        { 
            get 
            { 
                return _patient.MaritalStatus; 
            } 
            set
            {
                _patient.MaritalStatus = value;
            }
        }

        public Hl7.Fhir.Model.DataType Deceased 
        { 
            get 
            { 
                return _patient.Deceased; 
            } 
            set
            {
                _patient.Deceased = value;
            }
        }

        public FhirBoolean deceasedBoolean 
        { 
            get 
            { 
                return _patient.Deceased?.GetType() == typeof(FhirBoolean) ? (FhirBoolean)_patient.Deceased : null; 
            } 
            set
            {
                _patient.Deceased = value;
            }
        }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime? deceasedDateTime 
        { 
            get 
            { 
                return _patient.Deceased?.GetType() == typeof(FhirDateTime) ? Helper.IsoToDateTime(((FhirDateTime)_patient.Deceased).Value) : null; 
            } 
            set
            {
                _patient.Deceased = new FhirDateTime(value.ToString());
            }
        }

        public List<Patient.ContactComponent> Contact 
        { 
            get 
            { 
                return _patient.Contact; 
            } 
            set
            {
                _patient.Contact = value;
            }
        }

        public List<Patient.CommunicationComponent> Communication 
        { 
            get 
            { 
                return _patient.Communication; 
            }
            set
            {
                _patient.Communication = value; 
            }
        }

        public List<ResourceReference> GeneralPractitioner 
        { 
            get 
            { 
                return _patient.GeneralPractitioner; 
            }
            set
            {
                _patient.GeneralPractitioner = value;
            }
        }
    }
}
