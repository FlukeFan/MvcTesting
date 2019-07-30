using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using MvcTesting.Html;

namespace MvcTesting.Http
{
    public class Request
    {
        private const string _multipartBoundary = "---unit-_test___";

        public static readonly IDictionary<string, HttpStatusCode> DefaultStatusCodes = new Dictionary<string, HttpStatusCode>
        {
            { "GET",    HttpStatusCode.OK },
            { "POST",   HttpStatusCode.Redirect },
        };

        private string              _url;
        private string              _verb;
        private string              _query;
        private HttpStatusCode?     _expectedResponse;
        private NameValueCollection _headers            = new NameValueCollection();
        private IList<NameValue>    _formValues;
        private IList<FileUpload>   _fileUploads;

        public Request(string url, string verb = "GET")
        {
            var indexOfQuery = url.IndexOf('?');

            _url = url;

            if (indexOfQuery < 0)
                _query = null;
            else
                _query = url.Substring(indexOfQuery + 1);

            _verb = verb.ToUpper();

            if (DefaultStatusCodes.ContainsKey(_verb))
                _expectedResponse = DefaultStatusCodes[_verb];
        }

        public string                   Url                 { get { return _url; } }
        public string                   Verb                { get { return _verb; } }
        public HttpStatusCode?          ExptectedResponse   { get { return _expectedResponse; } }
        public NameValueCollection      Headers             { get { return _headers; } }
        public IEnumerable<NameValue>   FormValues          { get { return _formValues; } }

        public string Query()
        {
            if (_verb == "POST" || _formValues == null)
                return _query;

            var queryValues = _formValues.Select(nv => nv.UrlQueryValue());
            return string.Join("&", queryValues);
        }

        public Request StartForm()
        {
            _formValues = new List<NameValue>();
            _fileUploads = new List<FileUpload>();
            return this;
        }

        public Request SetExpectedResponse(HttpStatusCode? expectedResponseStatusCode)
        {
            _expectedResponse = expectedResponseStatusCode;
            return this;
        }

        public Request SetFormUrlEncoded()
        {
            SetHeader("Content-Type", "application/x-www-form-urlencoded");
            return this;
        }

        public Request SetMultipartForm()
        {
            SetHeader("Content-Type", $"multipart/form-data; boundary={_multipartBoundary}");
            return this;
        }

        public Request AddFormValue(string name, string value)
        {
            return AddFormValue(new NameValue(name, value));
        }

        public Request AddFileUpload(FileUpload fileUpload)
        {
            _fileUploads.Add(fileUpload);
            return this;
        }

        public Request AddFormValue(NameValue nameValue)
        {
            _formValues = _formValues ?? new List<NameValue>();
            _formValues.Add(nameValue);
            return this;
        }

        public Request SetHeader(string name, string value)
        {
            _headers[name] = value;
            return this;
        }

        public void SetContent(HttpRequestMessage netRequest)
        {
            if (_verb != "POST")
                return;

            if (_formValues == null)
                return;

            if (_fileUploads?.Count > 0)
            {
                SetMultipartForm();
                SetMultipartContent(netRequest);
            }
            else
            {
                SetFormUrlEncoded();
                SetFormContent(netRequest);
            }

            foreach (string name in Headers)
                netRequest.Content.Headers.Add(name, Headers[name]);
        }

        private void SetFormContent(HttpRequestMessage netRequest)
        {
            var sb = new StringBuilder();

            foreach (var formValue in FormValues)
                sb.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(formValue.Name), HttpUtility.UrlEncode(formValue.Value));

            var encodedFormValues = sb.ToString();
            var formBytes = Encoding.UTF8.GetBytes(encodedFormValues);
            netRequest.Content = new ByteArrayContent(formBytes);
        }

        private void SetMultipartContent(HttpRequestMessage netRequest)
        {
            using (var ms = new MemoryStream())
            {
                foreach (var formValue in _formValues)
                {
                    var sb = new StringBuilder();
                    sb.Append($"--{_multipartBoundary}");
                    sb.Append($"\r\nContent-Disposition: form-data; name=\"{formValue.Name}\"");
                    sb.Append($"\r\n\r\n{formValue.Value}\r\n");
                    var bytes = Encoding.ASCII.GetBytes(sb.ToString());
                    ms.Write(bytes, 0, bytes.Length);
                }

                foreach (var fileUpload in _fileUploads)
                {
                    var sb = new StringBuilder();
                    sb.Append($"--{_multipartBoundary}");
                    sb.Append($"\r\nContent-Disposition: form-data; name=\"{fileUpload.FormName}\"; filename=\"{fileUpload.FileName}\"");
                    sb.Append("\r\nContent-Type: application/x-object\r\n\r\n");
                    var bytes = Encoding.ASCII.GetBytes(sb.ToString());
                    ms.Write(bytes, 0, bytes.Length);
                    ms.Write(fileUpload.Content, 0, fileUpload.Content.Length);
                    bytes = Encoding.ASCII.GetBytes("\r\n");
                    ms.Write(bytes, 0, bytes.Length);
                }

                var final = $"--{_multipartBoundary}--";
                var finalBytes = Encoding.ASCII.GetBytes(final);
                ms.Write(finalBytes, 0, finalBytes.Length);

                var contentBytes = ms.ToArray();
                netRequest.Content = new ByteArrayContent(contentBytes);
            }
        }
    }
}
