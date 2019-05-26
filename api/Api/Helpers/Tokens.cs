
using Api.Models;
using Common;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Auth;

namespace Api.Helpers
{
	public class Tokens
	{
		public static async Task<LoginResult> GetLoginResult(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions)
		{
			return new LoginResult
			{
				Userid = identity.Claims.Single(c => c.Type == Constants.JwtClaimIdentifiers.Id).Value,
				Token = await jwtFactory.GetToken(userName, identity),
				Username = identity.Claims.Single(c => c.Type == Constants.JwtClaimIdentifiers.UserName).Value,
				Expiresin = (int)jwtOptions.ValidFor.TotalSeconds
			};
		}
	}
}
