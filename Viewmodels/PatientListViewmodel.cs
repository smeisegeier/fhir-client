using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Viewmodels
{
    public class PatientListViewmodel
    {
        public List<PatientEditViewmodel> EditViewmodelsCollection { get; set; }

        public PatientListViewmodel(List<Patient> patCol)
        {
            EditViewmodelsCollection = getViewmodelsFromPatients(patCol);
        }

        private List<PatientEditViewmodel> getViewmodelsFromPatients(List<Patient> patCol)
        {
            var list = new List<PatientEditViewmodel>();
            foreach (var item in patCol)
            {
                list.Add(new PatientEditViewmodel(item));
            }
            return list;
        }
    }
}
