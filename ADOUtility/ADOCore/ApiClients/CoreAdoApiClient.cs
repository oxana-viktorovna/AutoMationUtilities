using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;

namespace ADOCore.ApiClients
{
    public class CoreAdoApiClient
    {
        public CoreAdoApiClient(AdoSettings adoSettings)
        {
            client = new RestClient(adoSettings.BaseUrl)
            {
                Authenticator = new HttpBasicAuthenticator("", adoSettings.Password),
                Timeout = -1
            };
        }

        protected readonly RestClient client;

        protected IRestResponse SendAdoRequest(
            string resource,
            Method method = Method.GET,
            List<(string name, string value)> parameters = null,
            object body = null,
            string contentType = "application/json",
            string version = "6.0")
        {
            var request = BuildRequest(resource, method, parameters, body, contentType, version);
            var response = client.Execute(request);

            return response;
        }

        protected IRestResponse SendAdoPatchRequest(
            string resource,
            object body,
            string contentType = "application/json",
            string version = "6.0")
        {
            var request = BuildPatchRequest(resource, body, contentType, version);
            var n = request.ToString();
            var response = client.Execute(request);

            return response;
        }

        protected IRestResponse<T> SendAdoRequest<T>(
            string resource,
            Method method = Method.GET,
            List<(string name, string value)> parameters = null,
            object body = null,
            string contentType = "application/json",
            string version = "6.0")
        {
            var request = BuildRequest(resource, method, parameters, body, contentType, version);
            var response = client.Execute<T>(request);

            return response;
        }

        private IRestRequest BuildPatchRequest(string resource,
            object body,
            string contentType = "application/json",
            string version = "6.0")
        {
            var request = new RestRequest(resource, Method.PATCH);
            request.AddQueryParameter("api-version", version);
            request.AddHeader("Content-Type", contentType);
            request.RequestFormat = DataFormat.Json;

            request.AddJsonBody(body, contentType);
            request.AddOrUpdateParameter("Content-Type", contentType);

            return request;
        }

        private IRestRequest BuildRequest(
            string resource,
            Method method = Method.GET,
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

            if (method != Method.GET && body != null)
                request.AddJsonBody(body, contentType);

            return request;
        }
    }
}
