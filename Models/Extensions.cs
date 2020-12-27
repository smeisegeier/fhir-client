using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirClient.Models
{
    public static class Extensions
    {
        ///// <summary>
        ///// Returns object as json string
        ///// </summary>
        ///// <param name="baseObj">base object</param>
        ///// <returns>json</returns>
        //public static string ToJson(this Base baseObj) => new FhirJsonSerializer().SerializeToString(baseObj);

        ///// <summary>
        ///// Returns object as xml string
        ///// </summary>
        ///// <param name="baseObj"></param>
        ///// <returns>xml</returns>
        //public static string ToXml(this Base baseObj) => new FhirXmlSerializer().SerializeToString(baseObj);

        public static Base ToFhirBaseFromJson(this string json) => new FhirJsonParser().Parse(json); // FormatException
        public static Base ToFhirBaseFromXml(this string xml) => new FhirXmlParser().Parse(xml); // FormatException
    }
}
