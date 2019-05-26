using Common;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace Services
{
	public static class Extensions
	{

		public static string GetCurrentUserId()
		{
			if (Thread.CurrentPrincipal == null)
				return null;

			var name = ((ClaimsPrincipal)Thread.CurrentPrincipal).Claims.FirstOrDefault(x => x.Type == Constants.JwtClaimIdentifiers.Id);

            return name?.Value;
		}

	}

	public static class ExceptionExtensions
	{
		public static string InnermostMsg(this Exception e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}

			while (e.InnerException != null)
			{
				e = e.InnerException;
			}

			return e.Message;
		}
	}
}
