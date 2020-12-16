using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Models
{
    public interface IRepository
    {
        public List<Patient> GetPatients();
        public List<Patient> GetPatientsByMe();
        public Patient UpdatePatient(Patient patient);
        public Patient CreatePatient();

        /// <summary>
        /// Deletes Patient.
        /// </summary>
        /// <param name="id">patient object</param>
        /// <returns>success as bool</returns>
        public bool DeletePatient(string id);

        public List<Observation> GetObservations();
        public Observation GetObservation(string id);

        public Patient GetPatient(string id);
        public string GetJson(Resource res);
    }


    public class Repository : IRepository
    {
        private readonly string _baseUrl = "https://vonk.fire.ly/R4";

        public Repository() { }

        public List<Patient> GetPatients() => getAllResources(new List<Patient>(),20);

        public List<Patient> GetPatientsByMe()
        {
            // you may want to create searchparams..
            return getAllResources(new List<Patient>(),100).FindAll(i => i.Meta.Source == "dexterDSD");
        }


        public List<Observation> GetObservations() => getAllResources(new List<Observation>(), 20);

        public List<Organization> GetOrganizations() => getAllResources(new List<Organization>(), 20);


        public Patient UpdatePatient(Patient patient) => updateResource(cleansePatient(patient)) as Patient;

        public Patient CreatePatient() => createResource(initEmptyPatient()) as Patient;

        public Patient GetPatient(string id) => getResourceById(id, typeof(Patient)) as Patient;


        public bool DeletePatient(string id) => deleteResource(GetPatient(id));

        public Observation GetObservation(string id) => getResourceById(id, typeof(Observation)) as Observation;


        /// <summary>
        /// Gets the specified resource
        /// </summary>
        /// <param name="id">id as string</param>
        /// <param name="type">the desired type, eg: typeof(Patient)</param>
        /// <returns>Resource or null</returns>
        private Resource getResourceById(string id, Type type)
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

        // TODO delegates?
        private bool deleteResource(Resource resourceToBeDeleted)
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                try
                {
                    client.Delete(resourceToBeDeleted);
                    return true;
                }
                catch (FhirOperationException)
                {
                    return false;
                    //return Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound.ToString();
                }
            }
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


        private Resource updateResource(Resource resourceToBeUpdated)
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            {
                try
                {
                    var res = client.Update(resourceToBeUpdated);
                    return res;
                }
                catch (FhirOperationException)
                {
                    return null;
                }
            }
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
                    Family="Kelly",
                    Given = new List<string>() { "Joe", "Ralph"}
                }
            );
            pat.Name.Add(new HumanName()
                {
                    Use = HumanName.NameUse.Maiden,
                    Family = "Xantua"
                }
            );

            pat.Name.Add(new HumanName()
                {
                    Use = HumanName.NameUse.Nickname,
                    Family = "Wibbenhorst",
                    Given = new List<string>() { "Hans", "Dieter", "Hermann", "Walter"}
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




        private Resource getResourceFromJson(string json)
        {
            var parser = new FhirJsonParser();
            try
            {
                Resource parsedResource = parser.Parse<Resource>(json);
                return parsedResource;
            }
            catch (FormatException)
            {
                throw new FormatException();
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
                        .LimitTo(maxEntries)
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
