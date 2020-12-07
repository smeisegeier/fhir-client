using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Viewmodels
{
    public class ListViewmodel
    {
        public List<Patient> PatientsCollection { get; set; }

        public List<EditViewmodel> EditViewmodelsCollection { get; set; }

        public ListViewmodel(List<Patient> patCol)
        {
            PatientsCollection = patCol;
            UpdateCollection(patCol);
        }

        public void UpdateCollection(List<Patient> patCol)
        {
            EditViewmodelsCollection = new List<EditViewmodel>();
            foreach (var item in patCol)
            {
                EditViewmodelsCollection.Add(new EditViewmodel(item));
            }
        }
    }
}
