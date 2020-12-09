using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Viewmodels
{
    public class ObservationListViewmodel
    {
        public List<ObservationEditViewmodel> EditViewmodelsCollection { get; set; }

        public ObservationListViewmodel(List<Observation> patCol)
        {
            EditViewmodelsCollection = getViewmodelsFromObservations(patCol);
        }

        private List<ObservationEditViewmodel> getViewmodelsFromObservations(List<Observation> collection)
        {
            var list = new List<ObservationEditViewmodel>();
            foreach (var item in collection)
            {
                list.Add(new ObservationEditViewmodel(item));
            }
            return list;
        }
    }
}
