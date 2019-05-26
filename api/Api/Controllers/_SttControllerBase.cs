using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	public abstract class BloggingPlatformControllerBase : ControllerBase
	{
		protected const string UselessPassword = "UselessP@ssword1234";
	}
}
