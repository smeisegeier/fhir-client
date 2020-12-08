using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Viewmodels
{
    public class ListViewmodel
    {

        public List<EditViewmodel> EditViewmodelsCollection { get; set; }

        public ListViewmodel(List<Patient> patCol)
        {
            EditViewmodelsCollection = getViewmodelsFromPatients(patCol);
        }

        private List<EditViewmodel> getViewmodelsFromPatients(List<Patient> patCol)
        {
            var list = new List<EditViewmodel>();
            foreach (var item in patCol)
            {
                list.Add(new EditViewmodel(item));
            }
            return list;
        }
    }
}
