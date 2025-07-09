using RegistrationAndAuthorisationViaCookieMinimalAPI.DTO;
using RegistrationAndAuthorisationViaCookieMinimalAPI.Responses;

namespace RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Interfaces
{
    public interface ILoginManagement
    {

        Task<ResponseMessage<ResponseUser>> LoginUserAsync(HttpContext ctx, RequestUser user);
        Task<ResponseMessage<ResponseUser>> LogoutUserAsync(HttpContext ctx);
    }
}
