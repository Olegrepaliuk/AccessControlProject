using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AccessControl.Models
{
    public class RequestBuider
    {
        public static HttpRequestMessage GenerateHttpMessage(HttpMethod method, string uri, string username, string password)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(uri),
                Headers = {
                { HttpRequestHeader.Accept.ToString(), "application/json" },
                { "username", username},
                { "passhash", password},
            }
            };
            return httpRequestMessage;
        }

        public static HttpRequestMessage GenerateHttpMessageWithObj(HttpMethod method, string uri, string username, string password, object obj)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(uri),
                Headers = {
                { HttpRequestHeader.Accept.ToString(), "application/json" },
                { "username", username},
                { "password", password}
            },
            Content = new StringContent(JsonConvert.SerializeObject(obj))
            };
            return httpRequestMessage;
        }
    }
}
