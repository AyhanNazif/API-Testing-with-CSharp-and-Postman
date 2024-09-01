using BackendExam.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;

namespace BackendExam
{
    public class StorySpoil_Test
    {
        private RestClient client;
        private string baseURL = "https://d5wfqm7y6yb3q.cloudfront.net/api";
        private static string storyId;

        [SetUp]
        public void Setup()
        {
            string accessToken = GetAccessToken("guardian", "ignore");

            var options = new RestClientOptions(baseURL)
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            client = new RestClient(options);
        }

        private string GetAccessToken(string username, string password)
        {
            client = new RestClient(baseURL);
            var request = new RestRequest("/User/Authentication", Method.Post);

            request.AddJsonBody(new { username, password });

            var response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<UserAuthentication>(response.Content);

            return result.AccessToken;
        }

        [Test, Order(1)]
        public void CreateNewSpoiler_WithRequiredFields_Should_CreateSpoiler()
        {
            var request = new RestRequest("/Story/Create", Method.Post);

            request.AddJsonBody(new { title = "New Story Spoiler", description = "Some Story Description" });

            var response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.Content,Does.Contain("storyId"));
            Assert.That(result.Msg, Is.EqualTo("Successfully created!"));

            storyId = result.StoryId;

        }

        [Test, Order(2)]
        public void Edit_CreatedStory_Should_EditStory()
        {
            var request = new RestRequest($"/Story/Edit/{storyId}", Method.Put);

            request.AddJsonBody(new { title = "Updated Story Spoiler", description = "Updated Story Description" });

            var response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Msg, Is.EqualTo("Successfully edited"));

        }

        [Test, Order(3)]
        public void Delete_CreatedStory_Should_DeleteStory()
        {
            var request = new RestRequest($"/Story/Delete/{storyId}", Method.Delete);

            var response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Msg, Is.EqualTo("Deleted successfully!"));

        }

        [Test, Order(4)]
        public void CreateNewSpoiler_WithoutRequiredFields_Should_Return_BadRequest()
        {
            var request = new RestRequest("/Story/Create", Method.Post);

            request.AddJsonBody(new { title = "" });

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        }

        [Test, Order(5)]
        public void Edit_NonExistStory_Should_Return_NotFound()
        {
            var request = new RestRequest("/Story/Edit/1", Method.Put);

            request.AddJsonBody(new { title = "Non exist story Spoiler", description = "Non exist story Description" });

            var response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Msg, Is.EqualTo("No spoilers..."));

        }

        [Test, Order(6)]
        public void Delete_NonExistStory_Should_Return_BadRequest()
        {
            var request = new RestRequest("/Story/Delete/1", Method.Delete);

            var response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Msg, Is.EqualTo("Unable to delete this story spoiler!"));

        }
    }
}