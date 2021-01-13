using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EduLocate.Common
{
    /// <summary>Handles sending simple web requests and deserialising the response to a given type.</summary>
    public static class SimpleWebRequest
    {
        #region GetResponseFromEndpointAsync
        /// <summary>Sends a GET request asynchronously.</summary>
        /// <typeparam name="T">The type to deserialise the response to.</typeparam>
        /// <param name="uri">The uri to make the request to.</param>
        /// <param name="parameters">The parameters to add to the uri. The values are turned into strings via ToString before being encoded.</param>
        /// <returns>The servers response, of type T.</returns>
        public static async Task<T> GetResponseFromEndpointAsync<T>(string uri, IDictionary<string, object> parameters)
        {
            return await GetResponseFromEndpointAsync<T>(new Uri(uri), parameters);
        }

        /// <summary>Sends a GET request asynchronously.</summary>
        /// <typeparam name="T">The type to deserialise the response to.</typeparam>
        /// <param name="uri">The uri to make the request to.</param>
        /// <param name="parameters">The parameters to add to the uri. The values are turned into strings via ToString before being encoded.</param>
        /// <returns>The servers response, of type T.</returns>
        public static async Task<T> GetResponseFromEndpointAsync<T>(Uri uri, IDictionary<string, object> parameters)
        {
            Uri parameterisedUri = parameters == null ? uri : new Uri(uri, EncodeParameters(parameters));
            return await GetResponseFromEndpointAsync<T>(parameterisedUri);
        }

        /// <summary>Sends a GET request asynchronously.</summary>
        /// <typeparam name="T">The type to deserialise the response to.</typeparam>
        /// <param name="uri">The uri to make the request to.</param>
        /// <returns>The servers response, of type T.</returns>
        public static async Task<T> GetResponseFromEndpointAsync<T>(Uri uri)
        {
            WebRequest request = WebRequest.Create(uri);

            string content;
            using (WebResponse response = await request.GetResponseAsync())
            using (Stream responseStream = response.GetResponseStream())
            {
                if (responseStream is null)
                    content = string.Empty;
                else
                    content = await (new StreamReader(responseStream).ReadToEndAsync());
            }

            return JsonConvert.DeserializeObject<T>(content);
        }
        #endregion

        #region GetRedirectFromEndpointAsync

        /// <summary>Sends a GET request asynchronously and returns where the request was redirected to.</summary>
        /// <param name="uri">The uri to make the request to.</param>
        /// <param name="parameters">The parameters to add to the uri. The values are turned into strings via ToString before being encoded.</param>
        /// <returns>The uri the request was redirected to.</returns>
        public static async Task<string> GetRedirectFromEndpointAsync(string uri, IDictionary<string, object> parameters)
        {
            return await GetRedirectFromEndpointAsync(new Uri(uri), parameters);
        }

        /// <summary>Sends a GET request asynchronously and returns where the request was redirected to.</summary>
        /// <param name="uri">The uri to make the request to.</param>
        /// <param name="parameters">The parameters to add to the uri. The values are turned into strings via ToString before being encoded.</param>
        /// <returns>The uri the request was redirected to.</returns>
        public static async Task<string> GetRedirectFromEndpointAsync(Uri uri, IDictionary<string, object> parameters)
        {
            Uri parameterisedUri = parameters == null ? uri : new Uri(uri, EncodeParameters(parameters));
            return await GetRedirectFromEndpointAsync(parameterisedUri);
        }

        /// <summary>Sends a GET request asynchronously and returns where the request was redirected to.</summary>
        /// <param name="uri">The uri to make the request to.</param>
        /// <returns>The uri the request was redirected to.</returns>
        public static async Task<string> GetRedirectFromEndpointAsync(Uri uri)
        {
            WebRequest request = WebRequest.Create(uri);

            string redirectUri;
            using (WebResponse response = await request.GetResponseAsync())
            {
                redirectUri = response.ResponseUri.AbsoluteUri;
            }

            return redirectUri;
        }
        #endregion

        /// <summary>Handles transforming a dictionary of parameters into a string for a uri.</summary>
        /// <param name="parameters">The parameters to transform. The values are turned into strings via ToString.</param>
        /// <returns>A parameters string to use in a uri.</returns>
        private static string EncodeParameters(IDictionary<string, object> parameters)
        {
            return "?" + string.Join("&", parameters.Select(kv => $"{kv.Key}={kv.Value}"));
        }
    }
}
