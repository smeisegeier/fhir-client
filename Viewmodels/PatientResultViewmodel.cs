using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Viewmodels
{
    public class PatientResultViewmodel
    {
        public string Id { get; private set; }
        public bool Success { get; private set; }
        public List<ModelError> ListOfModelErrors { get; private set; }
        public OperationOutcome ListOfIssues { get; private set; }

        public PatientResultViewmodel(string id, bool success, List<ModelError> listOfModelErrors, OperationOutcome listOfIssues)
        {
            Id = id;
            Success = success;
            ListOfModelErrors = listOfModelErrors;
            ListOfIssues = listOfIssues;
        }
    }
}
