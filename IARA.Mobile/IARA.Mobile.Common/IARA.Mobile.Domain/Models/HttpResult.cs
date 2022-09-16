using System.Net;
using System.Text;

namespace IARA.Mobile.Domain.Models
{
    public class HttpResult
    {
        public HttpStatusCode StatusCode { get; }
        public ErrorModel Error { get; }

        public bool IsSuccessful { get; }

        public HttpResult(HttpStatusCode statusCode)
        {
            IsSuccessful = statusCode switch
            {
                HttpStatusCode.NoContent
                    or HttpStatusCode.Created
                    or HttpStatusCode.Accepted
                    or HttpStatusCode.OK => true,
                _ => false,
            };
            StatusCode = statusCode;
            Error = null;
        }

        public HttpResult(HttpStatusCode statusCode, ErrorModel error)
            : this(statusCode)
        {
            StatusCode = statusCode;
            Error = error;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder()
                .Append("StatusCode: ")
                .Append((int)StatusCode);

            return sb.ToString();
        }
    }

    public class HttpResult<T> : HttpResult
    {
        public T Content { get; }

        public string JsonResponse { get; }

        public HttpResult(HttpStatusCode statusCode)
            : base(statusCode)
        {
            Content = default;
        }

        public HttpResult(HttpStatusCode statusCode, T content)
            : base(statusCode)
        {
            Content = content;
        }

        public HttpResult(HttpStatusCode statusCode, ErrorModel error)
            : base(statusCode, error)
        {
            Content = default;
        }

        public HttpResult(HttpStatusCode statusCode, string json)
            : base(statusCode)
        {
            Content = default;
            JsonResponse = json;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder()
                .Append("StatusCode: ")
                .Append((int)StatusCode)
                .Append(", Content: ")
                .Append(Content == null ? "<null>" : Content.GetType().FullName);

            return sb.ToString();
        }
    }
}
