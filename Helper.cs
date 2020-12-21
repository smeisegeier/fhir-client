using FhirClient.Viewmodels;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FhirClient
{
    public static class Helper
    {
        public static string HtmlEncode(string s)
        {
            return s.Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace("\"", "&quot;");
        }

        public static DateTime? IsoToDateTime(string date)
        {
            if (string.IsNullOrEmpty(date))
                return null;
            if (date.Length == 8)
                date = string.Concat(date, "T00:00:00Z");
            return DateTime.Parse(date, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }

        public static string GetStringFromUrl(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                return webClient.DownloadString(url);
            }
        }

        public static string ReadTextFromFile(string fullPath) => File.ReadAllText(fullPath);


        // TODO improve
        public static void WriteTextToFile(IWebHostEnvironment webHostEnvironment, string text, string folderExDash, string extensionIncDot, string fileName = null)
        {
            string fileNameQ = fileName ?? Guid.NewGuid().ToString();
            string filePath = webHostEnvironment.WebRootPath + @"\" + folderExDash + @"\" + fileNameQ + extensionIncDot;
            File.WriteAllText(filePath, text);
        }
    }
}
