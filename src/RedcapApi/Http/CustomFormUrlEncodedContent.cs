using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Redcap.Http
{
    /// <summary>
    /// https://stackoverflow.com/a/23740338
    /// </summary>
    public class CustomFormUrlEncodedContent : ByteArrayContent
    {
        /// <summary>Creates a URL-encoded form body from the supplied keys and values.</summary>
        /// <param name="nameValueCollection">The form keys and values to encode.</param>
        public CustomFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
            : base(CustomFormUrlEncodedContent.GetContentByteArray(nameValueCollection))
        {
            base.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }
        /// <summary>Encodes the form pairs into the HTTP request body.</summary>
        /// <param name="nameValueCollection">The form keys and values to encode.</param>
        /// <returns>The encoded form bytes.</returns>
        private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException("nameValueCollection");
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> current in nameValueCollection)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append('&');
                }

                stringBuilder.Append(CustomFormUrlEncodedContent.Encode(current.Key));
                stringBuilder.Append('=');
                stringBuilder.Append(CustomFormUrlEncodedContent.Encode(current.Value));
            }
            return Encoding.Default.GetBytes(stringBuilder.ToString());
        }
        /// <summary>Escapes a single form key or value using form URL encoding.</summary>
        /// <param name="data">The unescaped form component.</param>
        /// <returns>The escaped component, or an empty string for null or empty input.</returns>
        private static string Encode(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            return System.Net.WebUtility.UrlEncode(data).Replace("%20", "+");
        }
    }
}