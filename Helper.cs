using FhirClient.Viewmodels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Copies IFormFile into given directory. Filename is retrieved from file. Dir will be created if not exists.
        /// </summary>
        /// <param name="file">given file</param>
        /// <param name="fullUploadDir">target directory on full path</param>
        public static void IFormFileToFile(IFormFile file, string fullUploadDir)
        {
            if (file.Length > 0)
            {
                if (!Directory.Exists(fullUploadDir))
                    Directory.CreateDirectory(fullUploadDir);
                using (var fileStream = new FileStream(Path.Combine(fullUploadDir, file.FileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
        }

        public static FileInfo[] GetFileInfoFromDirectory(string fullPath, string searchPattern = "*.*") => new DirectoryInfo(fullPath).GetFiles(searchPattern);
    }
}
