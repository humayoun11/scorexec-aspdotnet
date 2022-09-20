using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Authorization.Users;
using ScoringAppReact.Roles.Dto;
using ScoringAppReact.Users.Dto;

namespace ScoringAppReact.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<bool> ChangePassword(ChangePasswordDto input);

        Task<UserDetail> UserDetails(string contact);

        Task<User> GetUserByContact(string contact);
    }
}
