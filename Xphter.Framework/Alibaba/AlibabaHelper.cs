using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using Xphter.Framework.Collections;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Alibaba {
    /// <summary>
    /// Provides a utility for Alibaba library.
    /// </summary>
    public static class AlibabaHelper {
        private const string BASE_HTTP_URI_1688 = "http://gw.open.1688.com/openapi/";
        private const string BASE_HTTPS_URI_1688 = "https://gw.open.1688.com/openapi/";

        private static readonly Regex g_alibabaTimeRegex = new Regex(@"^(\d{4})(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])([01]\d|2[0-3])([0-5]\d)([0-5]\d)(\d{3})[-+](0\d|1[012])([0-5]\d)$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the relative URI path of API in open.1688.com.
        /// </summary>
        /// <param name="protocalVersion"></param>
        /// <param name="apiVersion"></param>
        /// <param name="apiNamespace"></param>
        /// <param name="apiName"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        private static string GetChinaApiRelativeUri(string protocalVersion, string apiVersion, string apiNamespace, string apiName, string appKey) {
            string urlPath = string.Empty;

            if(!string.IsNullOrWhiteSpace(protocalVersion)) {
                urlPath += protocalVersion;

                if(!string.IsNullOrWhiteSpace(apiVersion)) {
                    urlPath += "/" + apiVersion;

                    if(!string.IsNullOrWhiteSpace(apiNamespace)) {
                        urlPath += "/" + apiNamespace;

                        if(!string.IsNullOrWhiteSpace(apiName)) {
                            urlPath += "/" + apiName;

                            if(!string.IsNullOrWhiteSpace(appKey)) {
                                urlPath += "/" + appKey;
                            }
                        }
                    }
                }
            }

            return urlPath;
        }

        /// <summary>
        /// Gets the relative URI path of the absolute API URI in open.1688.com.
        /// </summary>
        /// <param name="absoluteApiUri"></param>
        /// <returns></returns>
        private static string GetChinaApiRelativeUri(string absoluteApiUri) {
            if(string.IsNullOrWhiteSpace(absoluteApiUri)) {
                throw new ArgumentException("The absolute API URI is null or empty.", "absoluteApiUri");
            }

            if(absoluteApiUri.StartsWith(BASE_HTTP_URI_1688, StringComparison.OrdinalIgnoreCase)) {
                return absoluteApiUri.Substring(BASE_HTTP_URI_1688.Length);
            } else if(absoluteApiUri.StartsWith(BASE_HTTPS_URI_1688, StringComparison.OrdinalIgnoreCase)) {
                return absoluteApiUri.Substring(BASE_HTTPS_URI_1688.Length);
            } else {
                throw new ArgumentException("The absolute API URI is invalid.", "absoluteApiUri");
            }
        }

        /// <summary>
        /// The internal method to create signature.
        /// </summary>
        /// <param name="urlPath"></param>
        /// <param name="args"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        private static string InternalGetSignature(string urlPath, IDictionary<string, string> args, string appSecret) {
            string cleartext = urlPath + args.Where((item) => !string.IsNullOrWhiteSpace(item.Value)).Select((item) => item.Key + item.Value).OrderBy((item) => item, StringComparer.Ordinal).StringJoin(string.Empty);
            byte[] ciphertext = new HMACSHA1(Encoding.UTF8.GetBytes(appSecret)).ComputeHash(Encoding.UTF8.GetBytes(cleartext));
            return ciphertext.StringJoin(string.Empty, (item) => item.ToString("X2"));
        }

        /// <summary>
        /// Gets argument dictionary from the specified args object.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IDictionary<string, string> GetArgumentsDictionary(object args) {
            if(args == null) {
                return new Dictionary<string, string>(0);
            }

            AlibabaArgumentAttribute argumentAttribute = null;
            ICollection<Element<string, object, AlibabaArgumentAttribute>> values = new List<Element<string, object, AlibabaArgumentAttribute>>();

            object propertyValue = null;
            foreach(PropertyInfo property in args.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where((item) => item.GetIndexParameters().Length == 0)) {
                if((propertyValue = property.GetValue(args, null)) == null) {
                    continue;
                }

                argumentAttribute = (AlibabaArgumentAttribute) property.GetCustomAttributes(typeof(AlibabaArgumentAttribute), false).FirstOrDefault();
                values.Add(new Element<string, object, AlibabaArgumentAttribute>(property.Name, propertyValue, argumentAttribute));
            }

            object fieldValue = null;
            foreach(FieldInfo field in args.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)) {
                if((fieldValue = field.GetValue(args)) == null) {
                    continue;
                }

                argumentAttribute = (AlibabaArgumentAttribute) field.GetCustomAttributes(typeof(AlibabaArgumentAttribute), false).FirstOrDefault();
                values.Add(new Element<string, object, AlibabaArgumentAttribute>(field.Name, fieldValue, argumentAttribute));
            }

            object objectValue = null;
            Type objectValueType = null;
            string formName = null, formValue = null;
            IAlibabaArgumentFormValueProvider formValueProvider = null;
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            IDictionary<string, string> arguments = new Dictionary<string, string>();
            foreach(Element<string, object, AlibabaArgumentAttribute> element in values) {
                objectValue = element.Component2;
                argumentAttribute = element.Component3;
                formValueProvider = argumentAttribute != null && argumentAttribute.FormValueProviderType != null ? (IAlibabaArgumentFormValueProvider) Activator.CreateInstance(argumentAttribute.FormValueProviderType) : null;

                formName = argumentAttribute != null && argumentAttribute.FormName != null ? argumentAttribute.FormName : element.Component1;

                if(formValueProvider != null) {
                    formValue = formValueProvider.GetFormValue(objectValue);
                } else {
                    objectValueType = objectValue.GetType();
                    if(objectValueType.IsString()) {
                        formValue = objectValue.ToString();
                    } else if(objectValueType.IsDateTime()) {
                        formValue = NetTimeToAlibabaTime((DateTime) objectValue);
                    } else if(objectValueType.IsBoolean()) {
                        formValue = objectValue.ToString().ToLower();
                    } else if(objectValueType.IsEnumerable()) {
                        formValue = jsonSerializer.Serialize(objectValue);
                        //formValue = jsonSerializer.Serialize(((IEnumerable) objectValue).Cast<object>().Where((item) => item != null).Select((item) => item.ToString()).ToArray());
                    } else {
                        formValue = objectValue.ToString();
                    }
                }

                arguments[formName] = formValue;
            }

            return arguments;
        }

        /// <summary>
        /// Gets query string of the specified arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetQueryString(IDictionary<string, string> args, Encoding encoding) {
            if(args == null || args.Count == 0) {
                return string.Empty;
            }

            return args.Where((item) => !string.IsNullOrWhiteSpace(item.Value)).Select((item) => string.Format("{0}={1}", HttpUtility.UrlEncode(item.Key, encoding), HttpUtility.UrlEncode(item.Value, encoding))).StringJoin('&');
        }

        /// <summary>
        /// Gets HTTP URI of API in open.1688.com.
        /// </summary>
        /// <param name="protocalVersion"></param>
        /// <param name="apiVersion"></param>
        /// <param name="apiNamespace"></param>
        /// <param name="apiName"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static string GetChinaApiHttpUri(string protocalVersion, string apiVersion, string apiNamespace, string apiName, string appKey) {
            return BASE_HTTP_URI_1688 + GetChinaApiRelativeUri(protocalVersion, apiVersion, apiNamespace, apiName, appKey);
        }

        /// <summary>
        /// Gets HTTPS URI of API in open.1688.com.
        /// </summary>
        /// <param name="protocalVersion"></param>
        /// <param name="apiVersion"></param>
        /// <param name="apiNamespace"></param>
        /// <param name="apiName"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static string GetChinaApiHttpsUri(string protocalVersion, string apiVersion, string apiNamespace, string apiName, string appKey) {
            return BASE_HTTPS_URI_1688 + GetChinaApiRelativeUri(protocalVersion, apiVersion, apiNamespace, apiName, appKey);
        }

        /// <summary>
        /// Gets signature of the specified arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string GetParamSignature(IDictionary<string, string> args, string appSecret) {
            return InternalGetSignature(null, args, appSecret);
        }

        /// <summary>
        /// Gets signature of the specified arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string GetParamSignature(object args, string appSecret) {
            return InternalGetSignature(null, GetArgumentsDictionary(args), appSecret);
        }

        /// <summary>
        /// Gets signature of the specified API and arguments.
        /// </summary>
        /// <param name="protocalVersion"></param>
        /// <param name="apiVersion"></param>
        /// <param name="apiNamespace"></param>
        /// <param name="apiName"></param>
        /// <param name="appKey"></param>
        /// <param name="args"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string GetApiSignature(string protocalVersion, string apiVersion, string apiNamespace, string apiName, string appKey, IDictionary<string, string> args, string appSecret) {
            return InternalGetSignature(GetChinaApiRelativeUri(protocalVersion, apiVersion, apiNamespace, apiName, appKey), args, appSecret);
        }

        /// <summary>
        /// Gets signature of the specified API and arguments.
        /// </summary>
        /// <param name="apiUri"></param>
        /// <param name="args"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string GetApiSignature(string apiUri, IDictionary<string, string> args, string appSecret) {
            return InternalGetSignature(GetChinaApiRelativeUri(apiUri), args, appSecret);
        }

        /// <summary>
        /// Gets signature of the specified API and arguments.
        /// </summary>
        /// <param name="protocalVersion"></param>
        /// <param name="apiVersion"></param>
        /// <param name="apiNamespace"></param>
        /// <param name="apiName"></param>
        /// <param name="appKey"></param>
        /// <param name="args"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string GetApiSignature(string protocalVersion, string apiVersion, string apiNamespace, string apiName, string appKey, object args, string appSecret) {
            return InternalGetSignature(GetChinaApiRelativeUri(protocalVersion, apiVersion, apiNamespace, apiName, appKey), GetArgumentsDictionary(args), appSecret);
        }

        /// <summary>
        /// Gets signature of the specified API and arguments.
        /// </summary>
        /// <param name="apiUri"></param>
        /// <param name="args"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string GetApiSignature(string apiUri, object args, string appSecret) {
            return InternalGetSignature(GetChinaApiRelativeUri(apiUri), GetArgumentsDictionary(args), appSecret);
        }

        /// <summary>
        /// Gets the OAuth URI of open.1688.com.
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="redirectUri"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string GetChinaOAuthUri(string appKey, string appSecret, string redirectUri, string state) {
            string signature = GetParamSignature(new Dictionary<string, string> {
                { "client_id", appKey },
                { "site", "china" },
                { "redirect_uri", redirectUri },
                { "state", state },
            }, appSecret);

            return string.Format("http://gw.open.1688.com/auth/authorize.htm?client_id={0}&site=china&redirect_uri={1}{2}&_aop_signature={3}",
                HttpUtility.UrlEncode(appKey),
                HttpUtility.UrlEncode(redirectUri),
                !string.IsNullOrWhiteSpace(state) ? "&state=" + HttpUtility.UrlEncode(state) : null,
                HttpUtility.UrlEncode(signature));
        }

        /// <summary>
        /// Converts .NET DateTime object to the Alibaba time format.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string NetTimeToAlibabaTime(DateTime time) {
            int zoneHour = 0, zoneMinute = 0;
            switch(time.Kind) {
                case DateTimeKind.Local:
                case DateTimeKind.Unspecified:
                    TimeZoneInfo local = TimeZoneInfo.Local;
                    zoneHour = local.BaseUtcOffset.Hours;
                    zoneMinute = local.BaseUtcOffset.Minutes;
                    break;
            }

            return time.ToString("yyyyMMddHHmmssfff") + (zoneHour > 0 ? "+" : "-") + Math.Abs(zoneHour).ToString("D2") + zoneMinute.ToString("D2");
        }

        /// <summary>
        /// Gets .NET DateTime from the Alibaba time value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime AlibabaTimeToLocalTime(string value) {
            Match match = g_alibabaTimeRegex.Match(value);
            if(match == null || !match.Success) {
                throw new ArgumentException("value not represents a valid alibaba time", "value");
            }

            DateTime time = new DateTime(
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value),
                int.Parse(match.Groups[3].Value),
                int.Parse(match.Groups[4].Value),
                int.Parse(match.Groups[5].Value),
                int.Parse(match.Groups[6].Value),
                int.Parse(match.Groups[7].Value),
                DateTimeKind.Unspecified);
            TimeZoneInfo sourceZone = TimeZoneInfo.CreateCustomTimeZone("alibaba", new TimeSpan(int.Parse(match.Groups[8].Value), int.Parse(match.Groups[9].Value), 0), "alibaba", "alibaba");
            TimeZoneInfo destinationZone = TimeZoneInfo.Local;

            time = TimeZoneInfo.ConvertTime(time, sourceZone, destinationZone);
            if(time.Kind != DateTimeKind.Local) {
                time = new DateTime(time.Ticks, DateTimeKind.Local);
            }

            return time;
        }
    }
}
