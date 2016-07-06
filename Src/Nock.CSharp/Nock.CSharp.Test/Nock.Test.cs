using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nock.CSharp.Test
{
    [TestClass]
    public class NockTest
    {
        [TestInitialize]
        public void PreTest()
        {
            Nock.CleanAll();
        }

		[TestMethod]
        public async Task MockOkGet()
        {
            var person = GetPerson();
            new Nock("http://localhost:8080").Get($"?id={person.Id}").Reply(HttpStatusCode.OK, person.ToJson());

            var client = GetHttpClient();
            var response = await client.GetAsync($"?id={person.Id}");

            var responsePerson = await GetContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(person.Id == responsePerson.Id);
        }

        [TestMethod]
        public async Task MockBadRequestGet()
        {
            var person = GetPerson();
            new Nock("http://localhost:8080").Get("/").Reply(HttpStatusCode.BadRequest, person.ToJson());

            var client = GetHttpClient();
            var response = await client.GetAsync("/");

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task MockOkPost()
        {
            var person = GetPerson();
            new Nock("http://localhost:8080").Post("/").Reply(HttpStatusCode.OK, person.ToJson());

            var client = GetHttpClient();
            var response = await client.PostAsync("/", GetPersonContent(person));

            var responsePerson = await GetContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(person.Id == responsePerson.Id);
        }

        [TestMethod]
        public async Task MockBadRequestPost()
        {
            var person = GetPerson();
            new Nock("http://localhost:8080").Post("/").Reply(HttpStatusCode.BadRequest, person.ToJson());

            var client = GetHttpClient();
            var response = await client.PostAsync("/", GetPersonContent(person));

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task MockOkPut()
        {
            var person = GetPerson();
            new Nock("http://localhost:8080").Put("/").Reply(HttpStatusCode.OK, person.ToJson());

            var client = GetHttpClient();
            var response = await client.PutAsync("/", GetPersonContent(person));

            var responsePerson = await GetContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(person.Id == responsePerson.Id);
        }

        [TestMethod]
        public async Task MockBadRequestPut()
        {
            var person = GetPerson();
            new Nock("http://localhost:8080").Put("/").Reply(HttpStatusCode.BadRequest, person.ToJson());

            var client = GetHttpClient();
            var response = await client.PutAsync("/", GetPersonContent(person));

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task MockOkPatch()
        {
            var person = GetPerson();
            new Nock("http://localhost:8080").Patch("/").Reply(HttpStatusCode.OK, person.ToJson());

            var client = GetHttpClient();
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), "/") {Content = GetPersonContent(person)};
            var response = await client.SendAsync(request);

            var responsePerson = await GetContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(person.Id == responsePerson.Id);
        }

        [TestMethod]
        public async Task MockBadRequestPatch()
        {
            var person = GetPerson();
            new Nock("http://localhost:8080").Patch("/").Reply(HttpStatusCode.BadRequest, person.ToJson());

            var client = GetHttpClient();
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), "/") {Content = GetPersonContent(person)};
            var response = await client.SendAsync(request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task MockOkDelete()
        {
            var person = GetPerson();
            new Nock("http://localhost:8080").Delete("/").Reply(HttpStatusCode.OK, person.ToJson());

            var client = GetHttpClient();
            var response = await client.DeleteAsync("/");

            var responsePerson = await GetContent(response);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(person.Id == responsePerson.Id);
        }

        [TestMethod]
        public async Task MockBadRequestDelete()
        {
            var person = GetPerson();
            new Nock("http://localhost:8080").Delete("/").Reply(HttpStatusCode.BadRequest, person.ToJson());

            var client = GetHttpClient();
            var response = await client.DeleteAsync("/");

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        [ExpectedException(typeof(WebException))]
        public async Task WebException()
        {
            new Nock("http://localhost:8080").Delete("/")
                .Reply(HttpStatusCode.OK, new WebException("An unknown error occurred"));

            var client = GetHttpClient();
            await client.DeleteAsync("/");
        }

        [TestMethod]
        [ExpectedException(typeof(NockException))]
        public async Task MissingUri()
        {
            var client = GetHttpClient();
            await client.GetAsync("/");
        }

        [TestMethod]
        [ExpectedException(typeof(NockException))]
        public async Task MissingMethod()
        {
            new Nock("http://localhost:8080").Delete("/").Reply(HttpStatusCode.OK, GetPerson().ToJson());

            var client = GetHttpClient();
            await client.GetAsync("/");
        }

        [TestMethod]
        [ExpectedException(typeof(NockException))]
        public async Task DuplicateMethods()
        {
            new Nock("http://localhost:8080").Get("/").Reply(HttpStatusCode.OK, GetPerson().ToJson());
            new Nock("http://localhost:8080").Get("/").Reply(HttpStatusCode.OK, GetPerson().ToJson());

            var client = GetHttpClient();
            await client.GetAsync("/");
        }

        private static Person GetPerson()
        {
            return new Person
            {
                FirstName = "Brandon",
                LastName = "Griffin"
            };
        }

        private static StringContent GetPersonContent(Person person)
        {
            return new StringContent(person.ToJson());
        }

        private async Task<Person> GetContent(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return content.FromJson<Person>();
        }

        private HttpClient GetHttpClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080")
            };
        }
    }
}