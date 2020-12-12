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
        private readonly string _baseUrl = "https://vonk.fire.ly/R4";

        public Repository() {        }

        public List<Patient> GetPatients()
        {
            return getAllResources(new List<Patient>());
        }

        public List<Patient> GetPatientsByMe()
        {
            SearchParams q = new SearchParams();
            q.Add("Meta.Source", "dexterDSD");
            return getAllResources(new List<Patient>(),20,q);
        }

        public List<Observation> GetObservations()
        {
            return getAllResources(new List<Observation>(), 20);
        }

        public List<Organization> GetOrganizations()
        {
            return getAllResources(new List<Organization>(), 20);
        }

        public Patient GetPatientById(string id)
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

        public string UpdatePatient(Patient patient)
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                try
                {
                    var updatedPatient = client.Update(patient);
                    return updatedPatient.Id;
                }
                catch (FhirOperationException)
                {
                    return string.Empty;
                    //return Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound.ToString();
                }
            }
        }

        public Patient CreatePatient(Patient newPat)
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {

                var res = client.Create(newPat);
                return res;
                //try
                //{
                //    var res = client.Create(newPat);
                //    return res;
                //}
                //catch (FhirOperationException)
                //{
                //    return null;
                //    //return Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound.ToString();
                //}
            }
        }


        public string DeletePatient(string id)
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                try
                {
                    client.Delete("Patient/" + id);
                    return id;
                }
                catch (FhirOperationException)
                {
                    return "";
                    //return Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound.ToString();
                }
            }
        }

        public Patient InitEmptyPatient()
        {
            var pat = new Patient();
            pat.Id = Guid.NewGuid().ToString();
            pat.Meta = new Meta();
            pat.Meta.Source = "dexterDSD";
            pat.Identifier = new List<Identifier>();
            pat.Identifier.Add(new Identifier());
            pat.MaritalStatus = new CodeableConcept();
            pat.MaritalStatus.Coding.Add(new Coding());
            pat.Communication = new List<Patient.CommunicationComponent>();
            pat.Communication.Add(new Patient.CommunicationComponent());
            pat.Contact = new List<Patient.ContactComponent>();
            pat.Contact.Add(new Patient.ContactComponent());
            pat.Address = new List<Address>();
            pat.Address.Add(new Address());
            pat.Name = new List<HumanName>();
            pat.Name.Add(new HumanName() 
                {   
                    Use= HumanName.NameUse.Official,
                    Family="default",
                    Given = new List<string>() { "Joe", "Ralph"}
                }
            );
            return pat;
        }


        public Observation GetObservationById(string id)
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                try
                {
                    return client.Read<Observation>("Observation/" + id);
                }
                catch (FhirOperationException)
                {
                    return null;
                    //return Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound.ToString();
                }
            }
        }

        // TODO check if makes sense
        public Resource GetResourceById(string id, Type type) 
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                try
                {
                    return client.Read<Resource>($"{nameof(type)}/" + id);
                }
                catch (FhirOperationException)
                {
                    return null;
                    //return Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound.ToString();
                }
            }
        }



        /// <summary>
        /// Get all resources from fhir server, move them to collection. Bases upon typ of given list
        /// </summary>
        /// <typeparam name="T">resource (e.g. patient)</typeparam>
        /// <param name="list">list to be filled</param>
        /// <param name="maxEntries"># of entries to be fetched</param>
        /// <returns></returns>
        private List<T> getAllResources<T>(List<T> list, int maxEntries=20, SearchParams q=null) where T : Resource
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                if (q is null)
                {
                    q = new SearchParams()
                        .LimitTo(maxEntries) // useless
                        .OrderBy("lastUpdated", SortOrder.Descending);
                }

                Bundle res = client.Search<T>(q);
                while (res != null && list.Count() < maxEntries)
                {
                    foreach (var item in res.Entry)
                    {
                        if (item.Resource.GetType() != typeof(OperationOutcome) && list.Count() < maxEntries)
                        {
                            var p = (T)item.Resource;
                            list.Add(p);
                        }
                    }
                    res = client.Continue(res, PageDirection.Next);
                }
                return list;
            }
        }

        public string GetJson(Resource res)
        {
            var xd = new Hl7.Fhir.Serialization.FhirJsonSerializer();
            return xd.SerializeToString(res);
        }

        public string GetJsonPatient(Patient pat)
        {
            var xd = new Hl7.Fhir.Serialization.FhirJsonSerializer();
            return xd.SerializeToString(pat);
        }
    }
}
