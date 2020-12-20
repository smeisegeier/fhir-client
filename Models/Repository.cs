using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FhirClient.Models
{
    public interface IRepository
    {
        public List<Patient> GetPatients();
        public List<Patient> GetPatientsByMe();
        public Patient UpdatePatient(Patient patient);
        public Patient CreatePatient();
        public Patient GetPatient(string id);
        public string GetPatientAsJson(Patient pat);
        public string GetPatientAsJson(string id);
        public string GetPatientAsXml(Patient pat);
        public string GetPatientAsXml(string id);

        /// <summary>
        /// Deletes Patient.
        /// </summary>
        /// <param name="id">patient object</param>
        /// <returns>success as bool</returns>
        public bool DeletePatient(string id);

        public List<Observation> GetObservations();
        public Observation GetObservation(string id);

        public ValueSet GetValueSet(string valueSetUrl);
        public CodeSystem GetCodeSystem(string codeSystemUrl); 

    }


    public class Repository : IRepository
    {
        //private const string _onto = "https://r4.ontoserver.csiro.au/fhir";
        //private const string _tx = "http://tx.fhir.org/r4";
        //private const string _snowVs = "http://snomed.info/sct?fhir_vs=refset/1072351000168102";
        //private const string _isoVs = "urn:iso:std:iso:3166";


        private const string _baseUrl = "https://vonk.fire.ly/R4"; 
        private const string _onto3 = "https://ontoserver.csiro.au/stu3-latest";


        public Repository()
        {
            initApplication();
        }

        /*   PATIENT   */
        public List<Patient> GetPatients() => getResources(new List<Patient>(),20);

        public List<Patient> GetPatientsByMe()
        {
            // you may want to create searchparams..
            return getResources(new List<Patient>(),100).FindAll(i => i.Meta.Source == "dexterDSD");
        }

        public Patient UpdatePatient(Patient patient) => updateResource(cleansePatient(patient)) as Patient;
        public Patient CreatePatient() => createResource(initEmptyPatient()) as Patient;
        public Patient GetPatient(string id) => getResourceById(id, typeof(Patient)) as Patient;
        public string GetPatientAsJson(Patient pat) => resourceToJson(pat);
        public string GetPatientAsJson(string id) => GetPatientAsJson(GetPatient(id));
        public string GetPatientAsXml(Patient pat) => resourceToXml(pat);
        public string GetPatientAsXml(string id) => GetPatientAsXml(GetPatient(id));
        public bool DeletePatient(string id) => deleteResource(GetPatient(id));

        /*   OBSERVATION   */

        public List<Observation> GetObservations() => getResources(new List<Observation>(), 20);
        public Observation GetObservation(string id) => getResourceById(id, typeof(Observation)) as Observation;

        /*   ORGANIZATION   */
        public List<Organization> GetOrganizations() => getResources(new List<Organization>(), 20);

        /*   TERMINOLOGY   */
        public ValueSet GetValueSet(string valueSetUrl) => jsonToBase(Helper.GetStringFromUrl(valueSetUrl)) as ValueSet;
        public CodeSystem GetCodeSystem(string codeSystemUrl) => jsonToBase(Helper.GetStringFromUrl(codeSystemUrl)) as CodeSystem;


        /*    private     */

        /// <summary>
        /// OneTime (static) setups
        /// </summary>
        private void initApplication()
        {
            // load static codesystems
            // patient-contactrelationship
            var cs = GetCodeSystem("https://hl7.org/FHIR/v2/0131/v2-0131.cs.canonical.json");
            Viewmodels.PatientEditViewmodel.CodeDropdownForContact = new SelectList(cs.Concept.ToList(), "Code", "Display");

            // TEST AREA - get ValueSets
            //var list = getResources(new List<ValueSet>(), 50, null, _baseUrl);
            //using (var client = new Hl7.Fhir.Rest.FhirClient(_baseUrl))
            //{
            //    // still unclear WHERE the vs is :o
            //    var res = client.ExpandValueSet(new FhirUri("https://hl7.org/FHIR/valueset-patient-contactrelationship.canonical.json"));
            //}

        }


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
                string g = "";
                foreach (string given in item.Given)
                {
                    g += given + " ";
                }
                item.Text = g + item.Family;
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
            pat.Name = new List<HumanName>()
            {
                new HumanName()
                {
                        Use= HumanName.NameUse.Official,
                        Family = "Arraya",
                        Given = new string[] { "Xilia", "yen" }
                },
                new HumanName()
                {
                        Use= HumanName.NameUse.Nickname,
                        Family = "Liston",
                        Given = new List<string> { "Fritz", "kolja" }
                },
            };

            pat.Identifier.Add(new Identifier()
                { 
                    Use = Identifier.IdentifierUse.Official,
                    System = "http://example.org", 
                    Value = "0815", 
                    Type = new CodeableConcept() 
                    {
                        Text = "Nice conecpt",
                        Coding = new List<Coding>()
                        {
                            new Coding()
                            {
                                System = "http://example.com/resource?foo=bar#fragment",
                                Code = "Syntax by sys",
                                Display = "Funky coding stuff"
                            }
                        }
                    }
                }
            );
            pat.Address.Add(new Address()
                {
                    Use = Address.AddressUse.Billing,
                    Country = "DE", City="Munich",
                    PostalCode= "55234",
                    Line = new string[] { "co", "Etage 4", "Heim"}
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


        private string resourceToJson(Resource resource) => new FhirJsonSerializer().SerializeToString(resource);
        private Base jsonToBase(string json) => new FhirJsonParser().Parse(json);
        private string resourceToXml(Resource resource) => new FhirXmlSerializer().SerializeToString(resource);



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
        private List<T> getResources<T>(List<T> list, int maxEntries, SearchParams q=null, string fhirUrl=_baseUrl) where T : Resource
        {
            using (var client = new Hl7.Fhir.Rest.FhirClient(fhirUrl))
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



    }
}
