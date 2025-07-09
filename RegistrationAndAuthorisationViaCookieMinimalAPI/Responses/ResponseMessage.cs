namespace RegistrationAndAuthorisationViaCookieMinimalAPI.Responses
{
    public class ResponseMessage<T>
    {

        public int StatusCode { get; set; } = 200;

        public string Message { get; set; } = string.Empty;

        public bool Success = false;

        public T Data { get; set; } = default!;

    }
}
