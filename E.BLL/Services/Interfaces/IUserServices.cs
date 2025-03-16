using Microsoft.AspNetCore.Mvc;
using Test02.Payload.Request;
using Test02.Payload.Response;

namespace E.BLL.Services.Interfaces
{
    public interface IUserServices
    {
        Task<PageResult<UserDTO>> GetUsers([FromQuery] PaginationParams paginationParams  , [FromQuery] UserReq req);

    }
}
