using System.Net;
using System.Text.Json.Serialization;

namespace EV.Model.Handlers
{
    public class ResponseModel<T>
    {
        public HttpStatusCode Status { get; set; }

        public bool IsSuccessed { get; set; }

        public T? Result { get; set; }

        public string Error { get; set; }

        [JsonConstructor]
        private ResponseModel(HttpStatusCode status, bool isSuccessed, T result, string error)
        {
            Status = status;
            IsSuccessed = isSuccessed;
            Result = result;
            Error = error;
        }

        public static ResponseModel<T> Success(T result)
        {
            return new ResponseModel<T>(HttpStatusCode.OK, true, result, null);
        }

        public static ResponseModel<T?> Failure(string error)
        {
            return new ResponseModel<T?>(HttpStatusCode.BadRequest, false, default, error);
        }
    }
}
