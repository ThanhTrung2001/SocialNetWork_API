using System.Text.Json.Serialization;

namespace EnVietSocialNetWorkAPI.Models
{
    public class ResponseModel<T>
    {
        public bool IsSuccessed { get; set; }

        public T Result { get; set; }

        public IEnumerable<string> Errors { get; set; }

        [JsonConstructor]
        private ResponseModel(bool isSuccessed, T result, IEnumerable<string> errors)
        {
            IsSuccessed = isSuccessed;
            Result = result;
            Errors = errors;
        }

        public ResponseModel<T> Success(T result)
        {
            return new ResponseModel<T>(true, result, new List<string>());
        }

        public ResponseModel<T?> Failure(IEnumerable<string> errors)
        {
            return new ResponseModel<T?>(false, default, errors);
        }
    }
}
