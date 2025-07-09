using RegistrationAndAuthorisationViaCookieMinimalAPI.DTO;
using RegistrationAndAuthorisationViaCookieMinimalAPI.Responses;

namespace RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Interfaces
{
    public interface IRegistrationManagement
    {
        Task<ResponseMessage<ResponseUser>> RegisterUserAsync(RequestUser user);
        Task<ResponseMessage<ResponseUser>> RemoveUserAsync(string email);
    }
}
