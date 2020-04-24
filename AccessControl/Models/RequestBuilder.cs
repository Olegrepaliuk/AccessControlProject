using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AccessControl.Models
{
    public class RequestBuilder
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
                { HttpRequestHeader.ContentType.ToString(), "application/json;charset=utf-8"},
                { "username", username},
                { "passhash", password}
            },
            Content = new StringContent(JsonConvert.SerializeObject(obj))
            };
            httpRequestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json;charset=utf-8");
            return httpRequestMessage;
        }
    }
}
