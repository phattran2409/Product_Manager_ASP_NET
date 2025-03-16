using AutoMapper;
using E.BLL.Services.Interfaces;

namespace E.BLL.Services
{
    public class UserServices(AppDbcontext appDbcontext, IMapper mapper) : IUserServices
    {
        public async Task<PageResult<UserDTO>> GetUsers([FromQuery] PaginationParams paginationParams, [FromQuery] UserReq req)
        {
            var query = appDbcontext.Users.AsQueryable();
            
            if (!string.IsNullOrEmpty(req.search))
            {
                query = query.Where(x => x.Name.Contains(req.search) || x.UserName.Contains(req.search) || x.Email.Contains(req.search));
            }
            if (!string.IsNullOrEmpty(req.status))
            {
                query = query.Where(x => x.status == req.status);
            }
            if (!string.IsNullOrEmpty(req.role))
            {
                query = query.Where(x => x.role == req.role);
            }
            var total = await query.CountAsync();
            var data = await query.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize).Take(paginationParams.PageSize).ToListAsync();
            
            return new PageResult<UserDTO>
            {
                Data = mapper.Map<List<UserDTO>>(data),
                CurrentPage = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalItems = total,      
               
            };
        }

    }
}
