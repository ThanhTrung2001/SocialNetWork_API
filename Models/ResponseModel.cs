using System.Text.Json.Serialization;

namespace EnVietSocialNetWorkAPI.Models
{
    public class ResponseModel<T>
    {
        public bool IsSuccessed { get; set; }

        public T Result { get; set; }

        public string Error { get; set; }

        [JsonConstructor]
        private ResponseModel(bool isSuccessed, T result, string error)
        {
            IsSuccessed = isSuccessed;
            Result = result;
            Error = error;
        }

        public static ResponseModel<T> Success(T result)
        {
            return new ResponseModel<T>(true, result, "");
        }

        public static ResponseModel<T?> Failure(string error)
        {
            return new ResponseModel<T?>(false, default, error);
        }
    }
}
