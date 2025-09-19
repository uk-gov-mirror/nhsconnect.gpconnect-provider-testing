using System;
using System.Collections.Generic;
using System.Configuration;
using GPConnect.Provider.AcceptanceTests.Logger;

// ReSharper disable ClassNeverInstantiated.Global

namespace GPConnect.Provider.AcceptanceTests.Helpers
{
    public class HttpHeaderHelper
    {
        private static bool IsDirectMode() =>
        "true".Equals(ConfigurationManager.AppSettings["directMode"], StringComparison.OrdinalIgnoreCase);

        private readonly Dictionary<string, string> _requestHeaders;

        public HttpHeaderHelper()
        {
            Log.WriteLine("HttpHeaderHelper() Constructor");
            _requestHeaders = new Dictionary<string, string>();
        }

        public void AddHeader(string key, string value)
        {
            // Block Authorization for direct-to-ALB runs
            if (IsDirectMode() && key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
            {
                RemoveHeader(key); // ensure it isn't present
                Log.WriteLine("DirectMode: Skipping Authorization header.");
                return;
            }

            _requestHeaders.Add(key, value);
            Log.WriteLine("Added Key='{0}' Value='{1}'", key, value);
        }

        public void ReplaceHeader(string key, string value)
        {
             if (IsDirectMode() && key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
            {
                RemoveHeader(key);
                Log.WriteLine("DirectMode: Skipping Authorization header.");
                return;
            }
            
            RemoveHeader(key);
            AddHeader(key, value);
            Log.WriteLine("Replaced Key='{0}' With Value='{1}'", key, value);
        }

        public void RemoveHeader(string key)
        {
            _requestHeaders.Remove(key);
            Log.WriteLine("Removed Key='{0}'", key);
        }

        public Dictionary<string, string> GetRequestHeaders()
        {
            Log.WriteLine("GetRequestHeaders Count='{0}'", _requestHeaders.Count);
            return _requestHeaders;
        }

        public void SetRequestHeaders(Dictionary<string, string> headers)
        {
            _requestHeaders.Clear();
            foreach (var header in headers) {
                _requestHeaders.Add(header.Key, header.Value);
            }
        }

        public string GetHeaderValue(string key)
        {
            string value;
            _requestHeaders.TryGetValue(key, out value);
            Log.WriteLine("Header Key='{0}' Value='{1}'", key, value);
            return value;
        }

        public void Clear()
        {
            _requestHeaders.Clear();
            Log.WriteLine("All Header(s) Cleared");
        }

    }
}
