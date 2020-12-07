using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Models
{
    public class Repository
    {
        public List<Patient> PatientsCollection { get; set; }

        private string _baseUrl = "https://vonk.fire.ly/R4";


        public Repository()
        {
        }

        public Patient GetPatientFromRepoById(string id)
        {
            return PatientsCollection.FirstOrDefault(i => i.Id == id);
        }

        public Patient GetPatientFromServerById(string id)
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                try
                {
                    return client.Read<Patient>("Patient/" + id);
                }
                catch (FhirOperationException)
                {
                    return null;
                    //return Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound.ToString();
                }
            }
        }

        /// <summary>
        /// Get all patients from fhir server, move them to collection
        /// </summary>
        /// <param name="maxEntries"># of entries to be fetched</param>
        public void GetAllPatientsFromServer(int maxEntries=10000)
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                var patientsCollection = new List<Patient>();
                var q = new SearchParams()
                    .LimitTo(2)
                    .OrderBy("Id", SortOrder.Ascending);
                //q.Add("gener", "male");
                Bundle res = client.Search<Patient>(q);
                while (res != null && patientsCollection.Count() < maxEntries)
                {
                    foreach (var item in res.Entry)
                    {
                        if (item.Resource.GetType() != typeof(OperationOutcome) && patientsCollection.Count() < maxEntries)
                        {
                            Patient p = (Patient)item.Resource;
                            patientsCollection.Add(p);
                        }
                    }
                    res = client.Continue(res, PageDirection.Next);
                }
                PatientsCollection = patientsCollection;
            }
        }

        public string GetJson(Patient pat)
        {
            var xd = new Hl7.Fhir.Serialization.FhirJsonSerializer();
            return xd.SerializeToString(pat);
        }

        private Patient createTestPatient()
        {
            var MyPatient = new Patient();
            MyPatient.Active = true;
            MyPatient.Identifier.Add(new Identifier() { System = "http://hl7.org/fhir/sid/us-ssn", Value = "000-12-3456" });
            MyPatient.Gender = AdministrativeGender.Male;
            MyPatient.Deceased = new FhirDateTime("2020-04-23");
            MyPatient.Name.Add(new HumanName()
            {
                Use = HumanName.NameUse.Official,
                Family = "Stokes",
                Given = new List<string>() { "Bran", "Deacon" },
                Period = new Period() { Start = "2015-05-12", End = "2020-02-15" }
            });
            MyPatient.AddExtension("http://hl7.org/fhir/StructureDefinition/patient-birthPlace", new Address() { Country = "US" });
            return MyPatient;
        }

    }
}
