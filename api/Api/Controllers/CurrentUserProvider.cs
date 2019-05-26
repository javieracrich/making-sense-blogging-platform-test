using Common;
using Domain;
using Microsoft.AspNetCore.Http;
using Services;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public class CurrentUserProvider : ICurrentUserProvider

    {
        private readonly IUserService userService;
        IHttpContextAccessor contextAccesor;

        public CurrentUserProvider(IUserService _userService, IHttpContextAccessor contextAccesor)
        {

            this.userService = _userService;
            this.contextAccesor = contextAccesor;
        }

        public async Task<User> GetCurrentAuthor()
        {
            var id = this.contextAccesor.HttpContext.User.Claims.First(x => x.Type == Constants.JwtClaimIdentifiers.Id).Value;
            var users = await this.userService.FindAsync<User>(x => x.Id == id);
            return users.FirstOrDefault();

        }
    }


    public interface ICurrentUserProvider
    {
        Task<User> GetCurrentAuthor();
    }
}
