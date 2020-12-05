using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Newtonsoft.Json;

namespace FhirClient
{
    public class Startup
    {
        private string _message = string.Empty;
        private Patient _patient;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    getRequest("https://vonk.fire.ly/R4/Patient/4");
                    if (_message != string.Empty)
                    {
                        _patient = JsonConvert.DeserializeObject<Patient>(_message);
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(_patient));
                    }
                });
            });
            postRequest("https://vonk.fire.ly/R4/Patient", JsonConvert.SerializeObject(createTestPatient()));
        }

        private async void getRequest(string baseUrl)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
            using (HttpContent content = res.Content)
            {
                string data = await content.ReadAsStringAsync();
                _message = data;
            }
        }

        private async void postRequest(string baseUrl, string jsonInString)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = new HttpResponseMessage())
            {
                await client.PostAsync(baseUrl, new StringContent(jsonInString, System.Text.Encoding.UTF8, "application/json"));
            }
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
            return MyPatient;
        }
    }
}
