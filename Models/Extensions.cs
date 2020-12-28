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
        /// <summary>
        /// Returns Base object of json string
        /// </summary>
        /// <param name="json">json string</param>
        /// <returns>base objecte</returns>
        public static Base ToFhirBaseFromJson(this string json) => new FhirJsonParser().Parse(json); // FormatException

        /// <summary>
        /// Returns Base object of xml string
        /// </summary>
        /// <param name="xml">xml</param>
        /// <returns>Base Object</returns>
        public static Base ToFhirBaseFromXml(this string xml) => new FhirXmlParser().Parse(xml); // FormatException
    }
}
