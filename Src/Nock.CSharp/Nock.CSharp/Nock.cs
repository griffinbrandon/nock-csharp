﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nock.CSharp
{
    public class Nocker
    {
        public Nocker(string basePath)
        {
            // check for trailing slash
            if (basePath.LastIndexOf('/') == basePath.Length - 1)
            {
                // trim off the trailing slash
                basePath = basePath.Substring(0, basePath.Length - 1);
            }

            BasePath = basePath;
            Nocks.Add(this);
        }

        public static List<Nocker> Nocks { get; } = new List<Nocker>();
        public string BasePath { get; set; }
        public string Uri { get; set; }
        public RequestType RequestType { get; set; }
        public HttpContent Content { get; set; }
        public Exception Exception { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public Nocker Get(string uri)
        {
            SetUri(uri);
            RequestType = RequestType.Get;
            return this;
        }

        public Nocker Patch(string uri)
        {
            SetUri(uri);
            RequestType = RequestType.Patch;
            return this;
        }

        public Nocker Post(string uri)
        {
            SetUri(uri);
            RequestType = RequestType.Post;
            return this;
        }

        public Nocker Put(string uri)
        {
            SetUri(uri);
            RequestType = RequestType.Put;
            return this;
        }

        public Nocker Delete(string uri)
        {
            SetUri(uri);
            RequestType = RequestType.Delete;
            return this;
        }

        public Nocker Head(string uri)
        {
            SetUri(uri);
            RequestType = RequestType.Head;
            return this;
        }

        public Nocker Merge(string uri)
        {
            SetUri(uri);
            RequestType = RequestType.Merge;
            return this;
        }

        public Nocker Reply(HttpStatusCode statusCode, HttpContent content)
        {
            StatusCode = statusCode;
            Content = content;
            return this;
        }

        public Nocker Reply(HttpStatusCode statusCode, string content)
        {
            var strContent = new StringContent(content);
            return Reply(statusCode, strContent);
        }

        public Nocker Reply(HttpStatusCode statusCode, string content, Encoding encoding)
        {
            var strContent = new StringContent(content, encoding);
            return Reply(statusCode, strContent);
        }

        public Nocker Reply(HttpStatusCode statusCode, string content, Encoding encoding, string mediaType)
        {
            var strContent = new StringContent(content, encoding, mediaType);
            return Reply(statusCode, strContent);
        }

        public Nocker Reply(HttpStatusCode statusCode, Exception exception)
        {
            StatusCode = statusCode;
            Exception = exception;
            return this;
        }

        public async Task<HttpResponseMessage> Respond()
        {
            return await Task.Run(() => GetResponse());
        }

        private void SetUri(string uri)
        {
            if (uri.IndexOf('/') != 0)
            {
                uri = '/' + uri;
            }
            Uri = uri;
        }

        private HttpResponseMessage GetResponse()
        {
            var response = new HttpResponseMessage(StatusCode);

            if (Content != null)
            {
                response.Content = Content;
                return response;
            }

            if (Exception != null)
            {
                throw Exception;
            }

            throw new NullReferenceException($"Unable to mock {BasePath}/{Uri}");
        }

        public static void CleanAll()
        {
            Nocks.Clear();
        }
    }
}