using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace APIClient
{
    public enum ClaimType
    {
        verify = 1,
        enrol = 2
    }

    public enum PhotoSource
    {
        eid,
        oid
    }

    public class APIClient
    {
        private readonly string baseURL;
        private readonly string apiKey;
        private readonly string secret;
        private readonly string appID;

        private readonly HttpClient httpClient = new HttpClient();

        public APIClient(string baseURL, string apiKey, string secret, string appID)
        {
            this.baseURL = baseURL;
            this.apiKey = apiKey;
            this.secret = secret;
            this.appID = appID;

            // User-Agent must always be sent, and Xamarin.Android doesn't send a
            // user agent for some reason
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Xamarin");
        }

        public async Task<string> GetToken(ClaimType type, string userID)
        {

            Dictionary<string, string> request = new Dictionary<string, string>
            {
                { "api_key", apiKey },
                { "secret", secret },
                { "resource", appID },
                { "client", "xamarin" },
                { "user_id", userID }
            };

            string json = JsonConvert.SerializeObject(request);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync($"{baseURL}/claim/{type}/token", content);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            Dictionary<string, object> responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            return (string) responseDict["token"];
        }

        // TODO: Better way to pass image than byte[]?
        public async Task<string> EnrolPhoto(string token, byte[] jpegImage, PhotoSource source)
        {
            var fileContent = new ByteArrayContent(jpegImage);

            MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
            multipartFormData.Add(new StringContent(apiKey), "api_key");
            multipartFormData.Add(new StringContent(secret), "secret");
            multipartFormData.Add(new StringContent("0"), "rotation");
            multipartFormData.Add(new StringContent(token), "token");
            multipartFormData.Add(fileContent, "image", "image.jpg");
            multipartFormData.Add(new StringContent(source.ToString()), "source");

            HttpResponseMessage response = await httpClient.PostAsync($"{baseURL}/claim/enrol/image", multipartFormData);

            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            Dictionary<string, object> responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            return (string)responseDict["token"];
        }
        
        // TODO: Turn into a proper ValidationResult
        public async Task<Dictionary<string, object>> Validate(string token, string userID)
        {
            Dictionary<string, string> request = new Dictionary<string, string>
            {
                { "api_key", apiKey },
                { "secret", secret },
                { "user_id", userID },
                { "token", token },
                { "ip", "127.0.0.1" },
                { "client", appID }
            };

            string json = JsonConvert.SerializeObject(request);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync($"{baseURL}/claim/verify/validate", content);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            Dictionary<string, object> responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            return responseDict;
        }

        public async Task<string> EnrolPhotoAndGetVerifyToken(string userID, byte[] jpegImage, PhotoSource source)
        {
            var enrolToken = await GetToken(ClaimType.enrol, userID);
            await EnrolPhoto(enrolToken, jpegImage, source);
            return await GetToken(ClaimType.verify, userID);
        }

    }

}
