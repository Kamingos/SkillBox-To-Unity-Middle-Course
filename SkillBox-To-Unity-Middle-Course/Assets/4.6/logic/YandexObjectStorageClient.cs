using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEngine.Networking;

namespace SkillBox.Course
{
    public sealed class YandexObjectStorageClient
    {
        private const string Host = "storage.yandexcloud.net";
        private const string Region = "ru-central1";
        private const string Algorithm = "AWS4-HMAC-SHA256";
        private const string SignedHeaders = "host;x-amz-content-sha256;x-amz-date";

        public IEnumerator Upload(
            string bucket, string key, string accessKey, string secret, byte[] json,
            Action<string> success, Action<string> failure)
        {
            string hash = Hash(json);
            string date;
            string url;
            string authorization = Sign("PUT", bucket, key, string.Empty, accessKey, secret, hash, out url, out date);

            using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
            {
                request.uploadHandler = new UploadHandlerRaw(json);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
                SetAuth(request, date, hash, authorization);
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                    success("JSON загружен в Object Storage.");
                else
                    failure(FormatError(request));
            }
        }

        public IEnumerator Download(
            string bucket, string key, string accessKey, string secret,
            Action<byte[]> success, Action<string> failure)
        {
            string hash = Hash(new byte[0]);
            string date;
            string url;
            string authorization = Sign("GET", bucket, key, string.Empty, accessKey, secret, hash, out url, out date);

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                SetAuth(request, date, hash, authorization);
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                    success(request.downloadHandler.data);
                else
                    failure(FormatError(request));
            }
        }

        public IEnumerator ListObjects(
            string bucket, string prefix, string accessKey, string secret,
            Action<string[]> success, Action<string> failure)
        {
            string query = "list-type=2&prefix=" + Uri.EscapeDataString(prefix);
            string hash = Hash(new byte[0]);
            string date;
            string url;
            string authorization = Sign(
                "GET", bucket, string.Empty, query, accessKey, secret, hash, out url, out date);

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                SetAuth(request, date, hash, authorization);
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    failure(FormatError(request));
                    yield break;
                }

                try
                {
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(request.downloadHandler.text);
                    XmlNodeList keys = document.GetElementsByTagName("Key");
                    List<string> result = new List<string>(keys.Count);
                    foreach (XmlNode node in keys)
                        result.Add(node.InnerText);
                    success(result.ToArray());
                }
                catch (XmlException exception)
                {
                    failure("Object Storage вернул некорректный XML: " + exception.Message);
                }
            }
        }

        private static string Sign(
            string method, string bucket, string key, string query, string accessKey, string secret,
            string payloadHash, out string url, out string timestamp)
        {
            string path = "/" + bucket + (string.IsNullOrEmpty(key) ? string.Empty : "/" + EncodeKey(key));
            url = "https://" + Host + path + (string.IsNullOrEmpty(query) ? string.Empty : "?" + query);

            DateTime now = DateTime.UtcNow;
            string day = now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            timestamp = now.ToString("yyyyMMdd'T'HHmmss'Z'", System.Globalization.CultureInfo.InvariantCulture);
            string scope = day + "/" + Region + "/s3/aws4_request";
            string headers = "host:" + Host + "\n" +
                             "x-amz-content-sha256:" + payloadHash + "\n" +
                             "x-amz-date:" + timestamp + "\n";
            string request = method + "\n" + path + "\n" + query + "\n" + headers + "\n" +
                             SignedHeaders + "\n" + payloadHash;
            string toSign = Algorithm + "\n" + timestamp + "\n" + scope + "\n" +
                            Hash(Encoding.UTF8.GetBytes(request));

            byte[] signingKey = Hmac(Hmac(Hmac(Hmac(
                Encoding.UTF8.GetBytes("AWS4" + secret), day), Region), "s3"), "aws4_request");
            string signature = Hex(Hmac(signingKey, toSign));
            return Algorithm + " Credential=" + accessKey + "/" + scope +
                   ", SignedHeaders=" + SignedHeaders + ", Signature=" + signature;
        }

        private static void SetAuth(UnityWebRequest request, string date, string hash, string authorization)
        {
            request.SetRequestHeader("x-amz-date", date);
            request.SetRequestHeader("x-amz-content-sha256", hash);
            request.SetRequestHeader("Authorization", authorization);
        }

        private static string EncodeKey(string key)
        {
            string[] parts = key.Split('/');
            for (int i = 0; i < parts.Length; i++)
                parts[i] = Uri.EscapeDataString(parts[i]);
            return string.Join("/", parts);
        }

        private static string Hash(byte[] bytes)
        {
            using (SHA256 sha = SHA256.Create())
                return Hex(sha.ComputeHash(bytes));
        }

        private static byte[] Hmac(byte[] key, string value)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
        }

        private static string Hex(byte[] bytes)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString("x2", System.Globalization.CultureInfo.InvariantCulture));
            return result.ToString();
        }

        private static string FormatError(UnityWebRequest request)
        {
            string response = request.downloadHandler == null ? string.Empty : request.downloadHandler.text;
            if (!string.IsNullOrWhiteSpace(response))
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(response);
                    XmlNodeList codes = document.GetElementsByTagName("Code");
                    XmlNodeList messages = document.GetElementsByTagName("Message");
                    string code = codes.Count == 0 ? string.Empty : codes[0].InnerText;
                    string message = messages.Count == 0 ? string.Empty : messages[0].InnerText;

                    if (!string.IsNullOrEmpty(code) || !string.IsNullOrEmpty(message))
                        return "Object Storage HTTP " + request.responseCode + " (" + code + "): " + message;
                }
                catch (XmlException)
                {
                    response = response.Replace('\r', ' ').Replace('\n', ' ').Trim();
                    if (response.Length > 300)
                        response = response.Substring(0, 300);
                    return "Object Storage HTTP " + request.responseCode + ": " + response;
                }
            }

            return "Object Storage HTTP " + request.responseCode + ": " + request.error;
        }
    }
}
