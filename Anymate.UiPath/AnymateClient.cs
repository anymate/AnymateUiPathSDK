using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Anymate.UiPath.Helpers;
using Anymate.UiPath.Models;
using Newtonsoft.Json;

namespace Anymate.UiPath
{
    public class AnymateClient
    {
        private static string AnymateUrl(string customerKey) => $"https://{customerKey}.anymate.app";
        private static string AnymateAuthUrl(string customerKey) => $"https://{customerKey}.auth.anymate.app";
        private static string OnPremisesApiUrl { get; set; }
        private static string OnPremisesAuthUrl { get; set; }
        private bool OnPremisesMode { get; set; } = false;
        private AuthTokenRequest _request { get; set; } = new AuthTokenRequest();

        public string AccessToken
        {
            get => _request.access_token;
            set => _request.access_token = value;
        }

        public bool HasAuthCredentials => AuthTokenRequestIsValid();

        private bool AuthTokenRequestIsValid()
        {
            if (_request == null)
                return false;

            if (string.IsNullOrWhiteSpace(_request.client_id))
                return false;

            if (string.IsNullOrWhiteSpace(_request.client_secret))
                return false;

            if (string.IsNullOrWhiteSpace(_request.password))
                return false;

            if (string.IsNullOrWhiteSpace(_request.username))
                return false;


            return true;
        }

        /// <summary>
        /// This is the default way of using the AnymateClient. It assumes you are using the cloud version of Anymate.
        /// </summary>
        /// <remarks>
        /// No need for any further configuration, as the Anymate Client automatically will setup itself once you log in.
        /// </remarks>
        public AnymateClient(string customerKey, string secret, string username, string password)
        {
            OnPremisesMode = false;
            _request = new AuthTokenRequest()
            {
                client_id = customerKey,
                client_secret = secret,
                password = password,
                username = username
            };
            GetOrRefreshAccessToken();
        }


        /// <summary>
        /// In order to run in OnPremisesMode, supply an Client Uri and Auth Uri
        /// </summary>
        /// <remarks>
        /// Supplying an Client Uri and Auth Uri will trigger the client to work in OnPremises mode, using the supplied url's instead of the cloud version of Anymate.
        /// </remarks>
        /// <param name="clientUri">The url for the anymate client installation. Url should just be the domain with http/https in front, similar to "https://customer.anymate.app"</param>
        /// <param name="authUri">The url for the anymate auth server installation. Url should just be the domain with http/https in front, similar to "https://customer.auth.anymate.app"</param>
        public AnymateClient(string customerKey, string secret, string username, string password, string clientUri,
            string authUri)
        {
            OnPremisesMode = true;
            _request = new AuthTokenRequest()
            {
                client_id = customerKey,
                client_secret = secret,
                password = password,
                username = username
            };
            clientUri = clientUri.Trim();
            if (clientUri.EndsWith("/"))
                clientUri = clientUri.Substring(0, clientUri.Length - 1);

            authUri = authUri.Trim();
            if (authUri.EndsWith("/"))
                authUri = authUri.Substring(0, authUri.Length - 1);

            OnPremisesApiUrl = clientUri;
            OnPremisesAuthUrl = authUri;
            GetOrRefreshAccessToken();
        }

        private string GetAnymateUrl(string customerKey)
        {
            if (OnPremisesMode)
            {
                return OnPremisesApiUrl;
            }
            else
            {
                return AnymateUrl(customerKey);
            }
        }

        private string GetAuthUrl(string customerKey)
        {
            if (OnPremisesMode)
            {
                return OnPremisesAuthUrl;
            }
            else
            {
                return AnymateAuthUrl(customerKey);
            }
        }


        private Dictionary<string, string> GetFormDataPasswordAuth(AuthTokenRequest request)
        {
            var values = CreateFormData(request);
            values.Add("grant_type", "password");
            values.Add(nameof(request.username), request.username);
            values.Add(nameof(request.password), request.password);
            return values;
        }

        private Dictionary<string, string> CreateFormData(AuthTokenRequest request)
        {
            var values = new Dictionary<string, string>();
            values.Add(nameof(request.client_id), request.client_id);
            values.Add(nameof(request.client_secret), request.client_secret);

            return values;
        }

        private void GetOrRefreshAccessToken()
        {
            var result = GetOrRefreshAccessToken(_request);
            if (!result.Succeeded)
                throw new Exception($"Could not authenticate. Got message: {result.HttpMessage}");
        }

        private AuthResponse GetOrRefreshAccessToken(AuthTokenRequest request)
        {

            if (string.IsNullOrWhiteSpace(request.access_token) || !TokenValidator.RefreshNotNeeded(request.access_token))
            {
                if (string.IsNullOrWhiteSpace(request.client_id))
                    throw new ArgumentNullException("client_id was null.");
                if (string.IsNullOrWhiteSpace(request.client_secret))
                    throw new ArgumentNullException("client_secret was null.");


                if (string.IsNullOrWhiteSpace(request.password) || string.IsNullOrWhiteSpace(request.username))
                {
                    throw new ArgumentNullException(
                        "Found no refresh token and either username or password were empty.");
                }

                var token = GetAuthTokenPasswordFlow(request);
                return token;
            }
            else
            {
                return new AuthResponse()
                {
                    access_token = _request.access_token,
                    Succeeded = true
                };
            }
        }

        private AuthResponse GetAuthTokenPasswordFlow(AuthTokenRequest request)
        {
            var formData = GetFormDataPasswordAuth(request);
            return GetAuthToken(formData, request.client_id);
        }
        
        private AuthResponse GetAuthToken(Dictionary<string, string> formData, string customerKey)
        {
            var values = formData;

            var content = new FormUrlEncodedContent(values);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMinutes(5);
                using (HttpResponseMessage response = AsyncUtil.RunSync(() =>
                    client.PostAsync($"{GetAuthUrl(customerKey)}/connect/token", content)))
                {
                    using (HttpContent responseContent = response.Content)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string data = AsyncUtil.RunSync(() => responseContent.ReadAsStringAsync());
                            var json = JsonConvert.DeserializeObject<AuthResponse>(data);
                            json.HttpMessage = $"{response.StatusCode} - {response.ReasonPhrase}";
                            _request.access_token = json.access_token;
                            return json;
                        }
                        else
                        {
                            var result = AuthResponse.Failed;
                            result.HttpMessage = $"{response.StatusCode} - {response.ReasonPhrase}";
                            _request.access_token = string.Empty;
                            return result;
                        }
                    }
                }
            }
        }

        private string GetCustomerKeyFromToken(string access_token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(access_token) as JwtSecurityToken;
            var customerKey = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "auth.anymate.app/CustomerKey")
                .Value;

            if (string.IsNullOrWhiteSpace(customerKey))
                throw new Exception("Token invalid");

            return customerKey;
        }

        private string CallApiPost(string endpoint, string jsonPayload)
        {
            GetOrRefreshAccessToken();
            var customerKey = GetCustomerKeyFromToken(_request.access_token);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _request.access_token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMinutes(5);
                using (HttpResponseMessage response = AsyncUtil.RunSync(() =>
                    client.PostAsync(GetAnymateUrl(customerKey) + endpoint, content)))
                using (HttpContent responseContent = response.Content)
                {
                    string data = AsyncUtil.RunSync(() => responseContent.ReadAsStringAsync());
                    return data;
                }
            }
        }

        private string CallApiGet(string endpoint)
        {
            GetOrRefreshAccessToken();
            var customerKey = GetCustomerKeyFromToken(_request.access_token);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _request.access_token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMinutes(5);
                using (HttpResponseMessage response =
                    AsyncUtil.RunSync(() => client.GetAsync(GetAnymateUrl(customerKey) + endpoint)))
                using (HttpContent responseContent = response.Content)
                {
                    string data = AsyncUtil.RunSync(() => responseContent.ReadAsStringAsync());
                    return data;
                }
            }
        }

        public TResponse Failure<TResponse>(string payload)
        {
            var endpoint = $"/apimate/Failure/";
            var response = CallApiPost(endpoint, payload);
            return JsonConvert.DeserializeObject<TResponse>(response);
        }

        public TResponse Failure<TResponse, TAction>(TAction action)
        {
            var payload = JsonConvert.SerializeObject(action);
            return Failure<TResponse>(payload);
        }


        public TResponse FinishRun<TResponse>(string payload)
        {
            var endpoint = $"/apimate/FinishRun/";
            var response = CallApiPost(endpoint, payload);
            return JsonConvert.DeserializeObject<TResponse>(response);
        }


        public TResponse FinishRun<TResponse, TAction>(TAction action)
        {
            var payload = JsonConvert.SerializeObject(action);
            return FinishRun<TResponse>(payload);
        }

        public T StartOrGetRun<T>(string processKey)
        {
            var endpoint = $"/apimate/StartOrGetRun/{processKey}";
            var response = CallApiGet(endpoint);
            return JsonConvert.DeserializeObject<T>(response);
        }

        public T OkToRun<T>(string processKey)
        {
            var endpoint = $"/apimate/OkToRun/{processKey}";
            var response = CallApiGet(endpoint);
            return JsonConvert.DeserializeObject<T>(response);
        }

        public string GetRules(string processKey)
        {
            var endpoint = $"/apimate/GetRules/{processKey}";
            return CallApiGet(endpoint);
        }

        public string TakeNext(string processKey)
        {
            var endpoint = $"/apimate/TakeNext/{processKey}";
            return CallApiGet(endpoint);
        }
        
        public TResponse CreateTask<TResponse>(string payload, string processKey)
        {
            var endpoint = $"/apimate/CreateTask/{processKey}";
            var response = CallApiPost(endpoint, payload);
            return JsonConvert.DeserializeObject<TResponse>(response);
        }

        public TResponse UpdateTask<TResponse>(string payload)
        {
            var endpoint = $"/apimate/UpdateTask/";

            var response = CallApiPost(endpoint, payload);
            return JsonConvert.DeserializeObject<TResponse>(response);
        }
        
        public string CreateAndTakeTask(string payload, string processKey)
        {
            var endpoint = $"/apimate/CreateAndTakeTask/{processKey}";
            var response = CallApiPost(endpoint, payload);
            return response;
        }

        public T Error<T>(string payload)
        {
            var endpoint = $"/apimate/Error/";
            var response = CallApiPost(endpoint, payload);
            return JsonConvert.DeserializeObject<T>(response);
        }
        
        public TResponse Error<TResponse, TAction>(TAction action)
        {
            var payload = JsonConvert.SerializeObject(action);
            return Error<TResponse>(payload);
        }




 
        public TResponse Retry<TResponse>(string payload)
        {
            var endpoint = $"/apimate/Retry/";
            var response = CallApiPost(endpoint, payload);
            return JsonConvert.DeserializeObject<TResponse>(response);
        }

        public TResponse Retry<TResponse, TAction>(TAction action)
        {
            var payload = JsonConvert.SerializeObject(action);
            return Retry<TResponse>(payload);
        }

 
        public TResponse Manual<TResponse>(string payload)
        {
            var endpoint = $"/apimate/Manual/";
            var response = CallApiPost(endpoint, payload);
            return JsonConvert.DeserializeObject<TResponse>(response);
        }


        public TResponse Manual<TResponse, TAction>(TAction action)
        {
            var payload = JsonConvert.SerializeObject(action);
            return Manual<TResponse>(payload);
        }
                
        public TResponse Solved<TResponse>(string payload)
        {
            var endpoint = $"/apimate/Solved/";
            var response = CallApiPost(endpoint, payload);
            return JsonConvert.DeserializeObject<TResponse>(response);
        }

        public TResponse Solved<TResponse, TAction>(TAction action)
        {
            var payload = JsonConvert.SerializeObject(action);
            return Solved<TResponse>(payload);
        }
    }
}