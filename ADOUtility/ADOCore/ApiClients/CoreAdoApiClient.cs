using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;

namespace ADOCore.ApiClients
{
    public class CoreAdoApiClient
    {
        public CoreAdoApiClient(AdoSettings adoSettings)
        {
            var options = new RestClientOptions(adoSettings.BaseUrl)
            {
                Authenticator = new HttpBasicAuthenticator("", adoSettings.Password),
                Timeout = System.TimeSpan.FromSeconds(3)
            };

            client = new RestClient(options);
            baseUrl = adoSettings.BaseUrl;
        }

        protected readonly RestClient client;
        protected readonly string baseUrl;

        protected RestResponse SendAdoRequest(
            string resource,
            Method method = Method.Get,
            List<(string name, string value)> parameters = null,
            object body = null,
            string contentType = "application/json",
            string version = "6.0")
        {
            var request = BuildRequest(resource, method, parameters, body, contentType, version);
            var response = client.Execute(request);

            return response;
        }

        protected RestResponse SendAdoPatchRequest(
            string resource,
            object body,
            string contentType = "application/json",
            string version = "6.0")
        {
            var request = BuildPatchRequest(resource, body, contentType, version);
            var response = client.Execute(request);

            return response;
        }

        protected RestResponse<T> SendAdoRequest<T>(
            string resource,
            Method method = Method.Get,
            List<(string name, string value)> parameters = null,
            object body = null,
            string contentType = "application/json",
            string version = "6.0")
        {
            var request = BuildRequest(resource, method, parameters, body, contentType, version);
            var response = client.Execute<T>(request);

            return response;
        }

        private RestRequest BuildPatchRequest(string resource,
            object body,
            string contentType = "application/json",
            string version = "6.0")
        {
            var request = new RestRequest(resource, Method.Patch);
            request.AddQueryParameter("api-version", version);
            request.AddHeader("Content-Type", contentType);
            request.RequestFormat = DataFormat.Json;

            request.AddJsonBody(body, contentType);
            request.AddOrUpdateParameter("Content-Type", contentType);

            return request;
        }

        private RestRequest BuildRequest(
            string resource,
            Method method = Method.Get,
            List<(string name, string value)> parameters = null,
            object body = null,
            string contentType = "",
            string version = "6.0")
        {
            var request = new RestRequest(resource, method);
            request.AddQueryParameter("api-version", version);
            if (parameters != null)
            {
                foreach (var (name, value) in parameters)
                {
                    request.AddQueryParameter(name, value);
                }
            }

            if (method != Method.Get && body != null)
                request.AddJsonBody(body, contentType);

            return request;
        }
    }
}
