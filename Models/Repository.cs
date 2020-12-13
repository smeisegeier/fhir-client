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
            return getAllResources(new List<Patient>(),20);
        }

        // TODO params dont work
        public List<Patient> GetPatientsByMe()
        {
            SearchParams q = new SearchParams();
            q.Add("Meta.Source", "dexterDSD");
            q.Add("Gender", "male");
            return getAllResources(new List<Patient>(),10,q);
        }

        public List<Observation> GetObservations()
        {
            return getAllResources(new List<Observation>(), 20);
        }

        public List<Organization> GetOrganizations()
        {
            return getAllResources(new List<Organization>(), 20);
        }


        private Patient cleansePatient(Patient pat)
        {
            // adopt text field
            foreach (HumanName item in pat.Name)
            {
                if (string.IsNullOrEmpty(item.Text))
                {
                    string g = "";
                    foreach (string given in item.Given)
                    {
                        g += given + " ";
                    }
                    item.Text = g + item.Family;
                }
            }
            return pat;
        }

        public string UpdatePatient(Patient patient)
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                try
                {
                    var updatedPatient = client.Update(cleansePatient(patient));
                    return updatedPatient.Id;
                }
                catch (FhirOperationException)
                {
                    return string.Empty;
                    //return Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound.ToString();
                }
            }
        }

        public Patient CreatePatient()
        {
            return (Patient)createResource(initEmptyPatient());
        }

        private Resource createResource(Resource resourceToBeCreated)
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                try
                {
                    var res = client.Create(resourceToBeCreated);
                    return res;
                }
                catch (FhirOperationException)
                {
                    return null;
                }
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

        private Patient initEmptyPatient()
        {
            var pat = new Patient();
            pat.Id = Guid.NewGuid().ToString();
            pat.Meta = new Meta();
            pat.Meta.Source = "dexterDSD";
            pat.Identifier = new List<Identifier>();
            pat.MaritalStatus = new CodeableConcept();
            pat.Communication = new List<Patient.CommunicationComponent>();
            pat.Contact = new List<Patient.ContactComponent>();
            pat.Contact.Add(new Patient.ContactComponent());
            pat.Address = new List<Address>();
            pat.Name = new List<HumanName>();
            pat.Name.Add(new HumanName() 
                {   
                    Use= HumanName.NameUse.Official,
                    Family="default",
                    Given = new List<string>() { "Joe", "Ralph"}
                }
            );
            pat.Identifier.Add(new Identifier()
                { 
                    Use = Identifier.IdentifierUse.Official,
                    System = "http://example.org", 
                    Value = "0815" 
                }
            );
            pat.Address.Add(new Address()
                {
                    Use = Address.AddressUse.Billing,
                    Country = "DE", City="Munich",
                    PostalCode= "55234"
                }
            );
            pat.MaritalStatus = new CodeableConcept()
            {
                Text = "xDE",
                Coding = new List<Coding>() 
                { 
                    new Coding() 
                    { 
                        Code = "U", Display="Unmarried", System="http://example.org"
                    } 
                }
            };
            pat.Communication.Add(new Patient.CommunicationComponent()
            {
                Preferred = true,
                Language = new CodeableConcept("http", "X", "myText")
            });
            pat.GeneralPractitioner.Add(new ResourceReference("GPReference","smt to display"));
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


        public Resource GetResourceById(string id, Type type) 
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                try
                {
                    var res = client.Read<Resource>($"{type.Name}/" + id);
                    //if (res is Patient)
                    //    res = improvePatient((Patient)res);
                    return res;
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
        private List<T> getAllResources<T>(List<T> list, int maxEntries, SearchParams q=null) where T : Resource
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

        //public string GetJsonPatient(Patient pat)
        //{
        //    var xd = new Hl7.Fhir.Serialization.FhirJsonSerializer();
        //    return xd.SerializeToString(pat);
        //}


        //public Patient GetPatientById(string id)
        //{
        //    using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
        //    {
        //        try
        //        {
        //            return improvePatient(client.Read<Patient>("Patient/" + id));
        //        }
        //        catch (FhirOperationException)
        //        {
        //            return null;
        //            //return Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound.ToString();
        //        }
        //    }
        //}
    }
}
