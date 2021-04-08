using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Viewmodels
{
    public class PatientEditViewmodel
    {
        public Patient _patient { get; private set; }

        public static SelectList CodeDropdownForContact { get; set; }

        // no parameterless ctor, hence on POST only the Model will be obtained
        public PatientEditViewmodel(Patient pat)
        {
            _patient = pat;
        }


        public string Id { get { return _patient.Id; } }

        public bool Active
        {
            get
            {
                if (_patient.Active == null || _patient.Active == true )
                    return true;
                else
                    return false;
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
                return DextersLabor.DateTimeHelper.IsoToDateTime(_patient.BirthDate);
            }
            set
            {
                _patient.BirthDate = value.ToString();
            }
        }



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
                return _patient.Deceased?.GetType() == typeof(FhirDateTime) ? DextersLabor.DateTimeHelper.IsoToDateTime(((FhirDateTime)_patient.Deceased).Value) : null; 
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

        public string lastUpdated { get => _patient.Meta?.LastUpdated.ToString(); }
    }
}
